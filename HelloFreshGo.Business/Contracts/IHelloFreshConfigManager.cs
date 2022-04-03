using System;
using System.Collections.Generic;
using System.Text;

namespace HelloFreshGo.Business.Contracts
{
    public interface IHelloFreshConfigManager
    {
        string AuthUsername { get; set; }
        string AuthPassword { get; set; }
        string HelloFreshConnection { get; set; }
    }
}
