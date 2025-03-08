using System;
using System.Threading.Tasks;
using ClearCare.Models.Entities;
using ClearCare.DataSource;

namespace ClearCare.Models.Control
{
     public class AdminAccountManagement
     {
          private readonly UserGateway _userGateway;

          // Constructor with dependency injection
          public AdminAccountManagement(UserGateway userGateway)
          {
               _userGateway = userGateway ?? throw new ArgumentNullException(nameof(userGateway));
          }
     }
}