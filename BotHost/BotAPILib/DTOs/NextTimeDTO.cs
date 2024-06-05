using BotApi.Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotAPILib.DTOs
{
    public class NextTimeDTO
    {
        public DateTime NextAvailableTime { get; set; }
        public Worker Worker { get; set; }
    }
}
