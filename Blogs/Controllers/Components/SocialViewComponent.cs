using Blogs.Enums;
using Blogs.Models;
using Blogs.ModelView;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blogs.Controllers.Components
{
    public class SocialViewComponent : ViewComponent
    {
        private readonly IConfiguration _config;
        private IMemoryCache _memoryCache;
        public SocialViewComponent(IConfiguration config, IMemoryCache memoryCache)
        {
            _config = config;
            _memoryCache = memoryCache;
        }

        public IViewComponentResult Invoke()
        {
            var _social = _memoryCache.GetOrCreate(CacheKeys.Social, entry =>
            {
                entry.SlidingExpiration = TimeSpan.MaxValue;
                return GetlsSocial();
            });
            return View(_social);
        }

        public SocialVM GetlsSocial()
        {
            SocialVM socialVM = new SocialVM();
            socialVM.Facebook = _config.GetValue<string>("SocialLinks:facebook");
            socialVM.Twitter = _config.GetValue<string>("SocialLinks:witter");
            socialVM.Instagram = _config.GetValue<string>("SocialLinks:instagram");
            socialVM.Pinterest = _config.GetValue<string>("SocialLinks:pinterest");
            socialVM.Youtube = _config.GetValue<string>("SocialLinks:youtube");
            return socialVM;
        }
    }
}
