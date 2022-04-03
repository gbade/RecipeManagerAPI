using HelloFreshGo.Business.Contracts;

namespace HelloFreshGo.Business.Managers
{
    public class HelloFreshConfigManager : IHelloFreshConfigManager
    {
        public string AuthUsername { get; set; }
        public string AuthPassword { get; set; }
        public string HelloFreshConnection { get; set; }
    }
}
