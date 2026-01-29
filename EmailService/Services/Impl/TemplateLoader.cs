namespace EmailService.Services.Impl
{
    public class TemplateLoader
    {
        private readonly string _basePath;
        private static readonly Dictionary<string, string> _cache = new();

        public TemplateLoader(IWebHostEnvironment env)
        {
            _basePath = Path.Combine(env.ContentRootPath, "Templates");
        }

        public void LoadAll()
        {
            if (!Directory.Exists(_basePath))
            {
                throw new DirectoryNotFoundException($"Templates folder not found at: {_basePath}");
            }

            var files = Directory.GetFiles(_basePath);

            foreach (var file in files)
            {
                var name = Path.GetFileName(file);
                _cache[name] = File.ReadAllText(file);
            }
        }

        public string Load(string name)
        {
            if (_cache.TryGetValue(name, out var template))
                return template;

            throw new Exception($"Template not loaded: {name}");
        }
    }

}
