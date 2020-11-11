using System;
using System.Collections.Generic;

using Logic.DTO.User;

namespace Logic.DTO
{
    public class BusinessQueueDTO
    {
        public BusinessDTO Business {get; set; }

        public int Capacity {get; set;}

        public DateTime Start { get; set; }

        public DateTime End { get; set; }
        
        public ICollection<UserDTO> Customers { get; set; }
    }
}