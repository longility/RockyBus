using Microsoft.Azure.Management.ServiceBus;
using Microsoft.Azure.Management.ServiceBus.Models;
using Microsoft.Rest.Azure;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockyBus.Azure.ServiceBus
{
    class ReceivingEndpoint
    {
        readonly AzureServiceBusConfiguration configuration;
        readonly IServiceBusManagementClient sbManagementClient;
        readonly string topicName;
        readonly IEnumerable<string> eventMessageTypeNames;

        public ReceivingEndpoint(AzureServiceBusConfiguration configuration,
                                 IServiceBusManagementClient sbManagementClient,
                                 string topicName,
                                 IEnumerable<string> eventMessageTypeNames)
        {
            this.configuration = configuration;
            this.sbManagementClient = sbManagementClient;
            this.topicName = topicName;
            this.eventMessageTypeNames = eventMessageTypeNames;
        }

        public async Task Initialize()
        {
            await CreateOrUpdateReceivingQueue().ConfigureAwait(false);
            await CreateOrUpdateForwardingSubscription().ConfigureAwait(false);

            var eventMessageFilter = CreateEventMessageFilter();
            var commandMessageFilter = new Rule
            {
                SqlFilter = new SqlFilter(
                    $"user.{UserProperties.DestinationQueueKey}='{configuration.ReceiveOptions.QueueName}'")
            };

            string name = nameof(eventMessageFilter);

            if (await IsDelta(eventMessageFilter))
            {
                await ClearExistingRules(name);
            }

            //429 too many requests if running rules at the same time
            int counter = 1;
            foreach (var r in eventMessageFilter)
            {
                var evtName = counter > 1 ? $"{name}{counter.ToString()}" : name;
                await CreateOrUpdateRule(evtName, r).ConfigureAwait(false);
                counter++;
            }
            await CreateOrUpdateRule(nameof(commandMessageFilter), commandMessageFilter).ConfigureAwait(false);
        }

        Task CreateOrUpdateReceivingQueue()
        {
            var sbQueue = configuration.ReceiveOptions.SBQueue ?? new SBQueue { };
            sbQueue.ForwardTo = null;
            return Retry.Do(() =>
                sbManagementClient.Queues.CreateOrUpdateAsync(
                    configuration.ResourceGroupName,
                    configuration.NamespaceName,
                    configuration.ReceiveOptions.QueueName,
                    sbQueue));
        }

        Task CreateOrUpdateForwardingSubscription()
        {
            var sbSubscription = configuration.ReceiveOptions.SBSubscription ?? new SBSubscription { };
            sbSubscription.ForwardTo = configuration.ReceiveOptions.QueueName.ToLower();
            return Retry.Do(() =>
                sbManagementClient.Subscriptions.CreateOrUpdateAsync(
                    configuration.ResourceGroupName,
                    configuration.NamespaceName,
                    topicName,
                    configuration.ReceiveOptions.QueueName,
                    sbSubscription));
        }

        IEnumerable<Rule> CreateEventMessageFilter()
        {
            List<Rule> result = new List<Rule>();
            if (!eventMessageTypeNames.Any())
                result.Add(new Rule { SqlFilter = new SqlFilter("user.alwaysfalse IS NOT NULL") });
            else
            {
                int maxCount = 10;
                int counter = 1;
                IEnumerable<string> typesNamesForFilter = eventMessageTypeNames.Take(maxCount);
                while (typesNamesForFilter.Any())
                {
                    result.Add(
                        new Rule
                        {
                            SqlFilter = new SqlFilter(
                                                $"user.{UserProperties.MessageTypeKey} IN ({string.Join(",", typesNamesForFilter.Select(n => $"'{n}'"))})")
                        }
                        );
                    typesNamesForFilter = eventMessageTypeNames.Skip(counter * maxCount).Take(maxCount);
                    counter++;
                }
            }
            return result;
        }

        Task CreateOrUpdateRule(string filterName, Rule rule) =>
            Retry.Do(() =>
                sbManagementClient.Rules.CreateOrUpdateAsync(
                    configuration.ResourceGroupName,
                    configuration.NamespaceName,
                    topicName,
                    configuration.ReceiveOptions.QueueName,
                    filterName,
                    rule)
            );

        async Task ClearExistingRules(string evtName)
        {
            try
            {
                var existingRules = await GetExistingRules();

                if (existingRules?.Any() == false) return;

                foreach (var rule in existingRules.Where(r => r.Name.Contains(evtName)))
                {
                    await sbManagementClient.Rules.DeleteAsync(
                           configuration.ResourceGroupName,
                           configuration.NamespaceName,
                           topicName,
                           configuration.ReceiveOptions.QueueName,
                           rule.Name);
                }
            }
            catch { }
        }

        async Task<IEnumerable<Rule>> GetExistingRules()
        {
            IPage<Rule> existingRules = await sbManagementClient.Rules.ListBySubscriptionsAsync(
                    configuration.ResourceGroupName,
                            configuration.NamespaceName,
                            topicName,
                            configuration.ReceiveOptions.QueueName);
            List<Rule> existingRulesList = existingRules.ToList();
            string nextPageLink = existingRules.NextPageLink;
            while (nextPageLink != null)
            {
                IPage<Rule> rules = await sbManagementClient.Rules.ListBySubscriptionsNextAsync(existingRules.NextPageLink);
                existingRulesList.AddRange(rules.ToList());
                nextPageLink = rules.NextPageLink;
            }
            return existingRulesList;
        }

        async Task<bool> IsDelta(IEnumerable<Rule> eventMessageTypes)
        {
            var existingMessageRules = await GetExistingRules();
            // Existing message rules has command message filter, so need to remove before comparing
            var existingMessageRulesCount = existingMessageRules.Any() ? existingMessageRules.Count() - 1 : 0;
            return eventMessageTypes.Count() != existingMessageRulesCount;
        }
    }
}
