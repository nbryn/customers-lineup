using System;
using System.Linq;

using Logic.BusinessQueues;
using Logic.Businesses;
using Logic.DTO.User;
using Logic.Users;
using Logic.DTO;

namespace Logic.Util
{
    public interface IDTOMapper
    {
       BusinessQueueDTO ConvertQueueToDTO(BusinessQueue queue);

       UserDTO ConvertUserToDTO(User user); 

       BusinessDTO ConvertBusinessToDTO(Business business);
    }
}