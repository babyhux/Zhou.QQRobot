using Autofac;
using Hangfire;
using Microsoft.Owin.Hosting;
using System;
using System.Threading.Tasks;

namespace Zhou.QQRobot.CQP.Hangfire
{
    public class OwinWebHost : IWebHost
    {
        private IDisposable _webHost;

        public Task StartAsync(string baseUrl, ILifetimeScope lifetimeScope)
        {
            _webHost = WebApp.Start(baseUrl, app => new Startup().Configuration(app, lifetimeScope));
            //RecurringJob.AddOrUpdate<RecurringJobs>((x) => x.KotowNotify(), "30 8 * * *", TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate<RecurringJobs>((x) => x.SignInNotify(), "30 9 * * *", TimeZoneInfo.Local);
            return Task.FromResult(0);
        }

        public Task StopAsync()
        {
            _webHost.Dispose();
            return Task.FromResult(0);
        }
    }
}
