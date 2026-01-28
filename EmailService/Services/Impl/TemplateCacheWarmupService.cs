namespace EmailService.Services.Impl
{
    public class TemplateCacheWarmupService : IHostedService
    {
        private readonly TemplateLoader _templateLoader;
        private readonly ILogger<TemplateCacheWarmupService> _logger;

        public TemplateCacheWarmupService(
            TemplateLoader templateLoader,
            ILogger<TemplateCacheWarmupService> logger)
        {
            _templateLoader = templateLoader;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Loading email templates into cache...");
            _templateLoader.LoadAll();
            _logger.LogInformation("Email templates loaded successfully.");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

}
