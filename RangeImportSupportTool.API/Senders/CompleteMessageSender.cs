﻿using Newtonsoft.Json;
using RangeImportSupportTool.APIService.Models;
using RangeImportSupportTool.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RangeImportSupportTool.APIService.Senders
{
    public class CompleteMessageSender
    {
        private readonly RangeImport _rangeImport;
        private readonly string _replyUrl;
        private readonly string _ticketUpdateUrl;
        private readonly string _replyJson;

        public CompleteMessageSender(RangeImport rangeImport)
        {
            _rangeImport = rangeImport;
            _replyUrl = ApiServiceHttpClient.BaseAddress + $"{_rangeImport.Id}/reply";
            _ticketUpdateUrl = ApiServiceHttpClient.BaseAddress + $"{_rangeImport.Id}";

            if (_rangeImport.IsNewReport)
            {
                _replyJson = $"{{\"body\": \"The range import has now been completed, the batch id is {_rangeImport.BatchId}.\"}}";
            }
            else
            {
                _replyJson = "{\"body\": \"The range import has now been completed.\"}";
            }
        }

        private async Task SendReply()
        {
            var replyContent = new StringContent(_replyJson, Encoding.UTF8, "application/json");

            await ApiServiceHttpClient.HttpClientReturn().PutAsync(_replyUrl, replyContent);
        }

        private async Task UpdateTicketStatus()
        {
            TicketUpdateJsonModel ticketUpdateJson = new()
            {
                Status = 4,
                Priority = 1,
                Tags = new[] { "RI", "Tasks" },
                Description = ".", 
                CustomFields = new CustomFields
                {
                    RootCauseReason = "Requests - Completed",
                    Application = "General Task", 
                    ImpactedArea = "Service/Work Request Area",
                    ResolvingTeam = "Application Support"
                }
            };

            var output = JsonConvert.SerializeObject(ticketUpdateJson);
            var content = new StringContent(output, Encoding.UTF8, "application/json");

            await ApiServiceHttpClient.HttpClientReturn().PutAsync(_ticketUpdateUrl, content);
        }
    }
}
