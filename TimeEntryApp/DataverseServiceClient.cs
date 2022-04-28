using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.PowerPlatform.Dataverse.Client.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeEntryApp
{
    public class DataverseServiceClient
    {
        public const string TimeEntryEntityName = "msdyn_timeentry";
        public const string MsdynStartAttributeName = "msdyn_start";
        public const string MsdynEndAttributeName = "msdyn_end";

        public const string DuplicateRuleName = "DuplicateRule: Time Entry with the same Start time";

        private readonly ServiceClient serviceClient;

        public DataverseServiceClient(ServiceClient serviceClient) {
            this.serviceClient = serviceClient;
        }

        public void CreateTimeEntryRecord(DateTime date)
        {
            var recordFileds = new Dictionary<string, DataverseDataTypeWrapper>();
            recordFileds.Add(MsdynStartAttributeName, new DataverseDataTypeWrapper(new DateTime(date.Year, date.Month, date.Day), DataverseFieldType.DateTime));
            recordFileds.Add(MsdynEndAttributeName, new DataverseDataTypeWrapper(date, DataverseFieldType.DateTime));

            var recordId = serviceClient.CreateNewRecord(TimeEntryEntityName, recordFileds, enabledDuplicateDetection: true);
        }

        private void EnableTimeEntryDuplicateRule()
        {
            var timeEntryEntity = (RetrieveEntityResponse) serviceClient.Execute(new RetrieveEntityRequest
            {
                RetrieveAsIfPublished = true,
                LogicalName = TimeEntryEntityName
            });
            if (!timeEntryEntity.EntityMetadata.IsDuplicateDetectionEnabled.Value)
            {
                timeEntryEntity.EntityMetadata.IsDuplicateDetectionEnabled = new BooleanManagedProperty(true);

                serviceClient.Execute(new UpdateEntityRequest
                {
                    Entity = timeEntryEntity.EntityMetadata
                });
            }
        }

        public void ConfgiureTimeEntryDuplicationRules()
        {
            EnableTimeEntryDuplicateRule();

            Guid ruleId;
            var dupRule = GetExistingDuplicateRule() as DuplicateRule;

            // Create duplication rule if not exists
            if(dupRule == null)
            {
                var timeEntryDuplicateRule = new DuplicateRule
                {
                    Name = DuplicateRuleName,
                    BaseEntityName = TimeEntryEntityName,
                    MatchingEntityName = TimeEntryEntityName
                };
                ruleId = serviceClient.Create(timeEntryDuplicateRule);
            } 
            else
            {
                ruleId = dupRule.Id;
            }

            // Create duplication rule condition if not exists
            var dupRuleCondition = GetExistingDuplicateRuleCondition(ruleId);
            if(dupRuleCondition == null)
            {
                DuplicateRuleCondition accountDupCondition = new DuplicateRuleCondition
                {
                    BaseAttributeName = MsdynStartAttributeName,
                    MatchingAttributeName = MsdynStartAttributeName,
                    OperatorCode = duplicaterulecondition_operatorcode.SameDate,
                    RegardingObjectId = new EntityReference(DuplicateRule.EntityLogicalName, ruleId)
                };

                Guid conditionId = serviceClient.Create(accountDupCondition);
            }

            // Publish and check
            if(dupRule == null || dupRule.StatusCode.Value != 2)
            {
                var response =
                (PublishDuplicateRuleResponse)serviceClient.Execute(new PublishDuplicateRuleRequest() { DuplicateRuleId = ruleId });

                DuplicateRule retrievedRule;
                int i = 0;
                do
                {
                    i++;
                    System.Threading.Thread.Sleep(1000);
                    retrievedRule =
                        (DuplicateRule)serviceClient.Retrieve(DuplicateRule.EntityLogicalName, ruleId, new ColumnSet(new String[] { "statuscode" }));
                } while (retrievedRule.StatusCode.Value == 1 && i < 20);
            }
        }

        private Entity GetExistingDuplicateRuleCondition(Guid ruleId)
        {
            QueryExpression dupRuleQuery = new QueryExpression
            {
                EntityName = DuplicateRuleCondition.EntityLogicalName,
                ColumnSet = new ColumnSet("matchingattributename"),
                Criteria = new FilterExpression
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions =
                    {
                        new ConditionExpression
                        {
                            AttributeName = "regardingobjectid",
                            Operator = ConditionOperator.Equal,
                            Values = { ruleId }
                        },
                        new ConditionExpression
                        {
                            AttributeName = "matchingattributename",
                            Operator = ConditionOperator.Equal,
                            Values = { MsdynStartAttributeName }
                        },
                        new ConditionExpression
                        {
                            AttributeName = "operatorcode",
                            Operator = ConditionOperator.Equal,
                            Values = { (int)duplicaterulecondition_operatorcode.SameDate }
                        }
                    }
                },
            };

            var resp = serviceClient.RetrieveMultiple(dupRuleQuery);

            return resp.Entities.FirstOrDefault();
        }

        private Entity GetExistingDuplicateRule()
        {
            QueryExpression dupRuleQuery = new QueryExpression
            {
                EntityName = DuplicateRule.EntityLogicalName,
                ColumnSet = new ColumnSet("name", "statuscode"),
                Criteria = new FilterExpression
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions =
                    {
                        new ConditionExpression
                        {
                            AttributeName = "name",
                            Operator = ConditionOperator.Equal,
                            Values = { DuplicateRuleName }
                        }
                    }
                },
            };

            var resp = serviceClient.RetrieveMultiple(dupRuleQuery);

            return resp.Entities.FirstOrDefault();
        }
    }
}
