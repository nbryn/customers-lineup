using System;
using System.Collections.Generic;

using Logic.DTO.User;

namespace Logic.DTO
{
    public class BusinessQueueDTO
    {
        public int BusinessId { get; set; }
        public DateTime Start { get; set; }

        public DateTime End { get; set; }

    }
}