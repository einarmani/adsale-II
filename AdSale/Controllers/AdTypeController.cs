using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdSale.Helpers;
using AdSale.Models;
using AdSale.ServiceModels;
using AdSale.Services;
using Amazon.S3;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AdSale.Controllers
{
    public class AdTypeController : Controller
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IMapper _mapper;

        public AdTypeController(IAmazonS3 s3Client, IMapper mapper)
        {
            _s3Client = s3Client;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var awsService = new AwsService<ICollection<AdTypeModel>>(_s3Client, AdSaleConstants.ConfigKey);
            var model = await awsService.GetObject(AdSaleConstants.TypeObjectKey);

            if (model != null)
            {
                var media = GetMedia();
                if (media.Result != null)
                {
                    foreach (var item in model)
                    {
                        var m = Enumerable.FirstOrDefault(media.Result, x => x.Id == item.MediaId);
                        if (m != null)
                        {
                            item.MediaName = m.Name;
                        }
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new AdTypeModel();
            var media = GetMedia();
            if (media.Result == null)
            {
                // no media registered so we cannot allow the creation of adsale types
                return View("MediaMissing");
            }

            model.Media = media.Result;
            model.IsActive = true;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdTypeModel model)
        {
            if (ModelState.IsValid)
            {
                var awsService = new AwsService<ICollection<AdType>>(_s3Client, AdSaleConstants.ConfigKey);
                var existingTypes = await awsService.GetObject(AdSaleConstants.TypeObjectKey) ?? new List<AdType>();

                var typeToSave = _mapper.Map<AdType>(model);
                typeToSave.Id = IdGenerator.Make(existingTypes.Select(x => x.Id).ToList());

                existingTypes.Add(typeToSave);

                await awsService.SaveObject(existingTypes, AdSaleConstants.TypeObjectKey);

                return RedirectToAction("Index");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var awsService = new AwsService<ICollection<AdTypeModel>>(_s3Client, AdSaleConstants.ConfigKey);
            var existingTypes = await awsService.GetObject(AdSaleConstants.TypeObjectKey);
            var adType = existingTypes.First(x => x.Id == id);
            var media = GetMedia();
            var model = _mapper.Map<AdTypeModel>(adType);
            model.Media = media.Result;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdTypeModel model)
        {
            if (ModelState.IsValid)
            {
                var awsService = new AwsService<ICollection<AdType>>(_s3Client, AdSaleConstants.ConfigKey);
                var existingTypes = await awsService.GetObject(AdSaleConstants.TypeObjectKey);

                var adSaleType = existingTypes.First(x => x.Id == model.Id);
                adSaleType.Name = model.Name;
                adSaleType.AdditionalCharacterCount = model.AdditionalCharacterCount;
                adSaleType.AdditionalCharacterPrice = model.AdditionalCharacterPrice;
                adSaleType.BaseCharacterCount = model.BaseCharacterCount;
                adSaleType.BaseCharacterPrice = model.BaseCharacterPrice;
                adSaleType.HeaderPrice = model.HeaderPrice;
                adSaleType.MediaId = model.MediaId;
                adSaleType.IsActive = model.IsActive;

                await awsService.SaveObject(existingTypes, AdSaleConstants.TypeObjectKey);

                return RedirectToAction("Index");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        #region *** Private Methods ***
        private async Task<ICollection<MediaModel>> GetMedia()
        {
            var awsService = new AwsService<ICollection<MediaModel>>(_s3Client, AdSaleConstants.ConfigKey);
            var result = await awsService.GetObject(AdSaleConstants.MediaObjectKey);

            return result;
        }

        #endregion
    }
}