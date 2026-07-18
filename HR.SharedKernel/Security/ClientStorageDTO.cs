using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.SharedKernel.Security
{
  public  class ClientStorageDTO
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string UserFullPersianName { get; set; }
        public DateTime expiresOn { get; set; }
        public DateTime RefreshTokenExpiresOn { get; set; }
        public bool RequiresPasswordChange { get; set; }
    }
}
