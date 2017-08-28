using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdSale.Helpers;
using AdSale.Models;
using AdSale.ServiceModels;
using AdSale.Services;
using Amazon.S3;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdSale.Controllers
{
    // todo: create a new role
    //[Authorize(Roles = "Admin")]
    public class MediaController : Controller
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IMapper _mapper;

        public MediaController(IAmazonS3 s3Client, IMapper mapper)
        {
            _s3Client = s3Client;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var awsService = new AwsService<ICollection<Media>>(_s3Client, AdSaleConstants.ConfigKey);

            var result = await awsService.GetObject(AdSaleConstants.MediaObjectKey);

            //var result = new List<Media>();
            //result.Add(new Media
            //{
            //    Id = "123",
            //    IsActive = true,
            //    Name = "visir.is"
            //});
            //result.Add(new Media
            //{
            //    Id = "456",
            //    IsActive = true,
            //    Name = "frettabladid.is"
            //});
            
            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            // make sure the visible flag is true by default
            var model = new MediaModel { IsActive = true };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MediaModel model)
        {
            if (ModelState.IsValid)
            {
                var awsService = new AwsService<ICollection<Media>>(_s3Client, AdSaleConstants.ConfigKey);
                var existingItems = await awsService.GetObject(AdSaleConstants.MediaObjectKey) ?? new List<Media>();

                var mediaToSave = new Media
                {
                    Id = IdGenerator.Make(existingItems.Select(x => x.Id).ToList()),
                    Name = model.Name,
                    IsActive = model.IsActive
                };

                existingItems.Add(mediaToSave);

                await awsService.SaveObject(existingItems, AdSaleConstants.MediaObjectKey);

                return RedirectToAction("Index");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var awsService = new AwsService<ICollection<Media>>(_s3Client, AdSaleConstants.ConfigKey);
            var existingItems = await awsService.GetObject(AdSaleConstants.MediaObjectKey);
            var item = existingItems.First(x => x.Id == id);
            var model = _mapper.Map<MediaModel>(item);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MediaModel model)
        {
            if (ModelState.IsValid)
            {
                var awsService = new AwsService<ICollection<Media>>(_s3Client, AdSaleConstants.ConfigKey);
                var existingItems = await awsService.GetObject(AdSaleConstants.MediaObjectKey);

                var item = existingItems.First(x => x.Id == model.Id);
                item.IsActive = model.IsActive;
                item.Name = model.Name;

                await awsService.SaveObject(existingItems, AdSaleConstants.MediaObjectKey);

                return RedirectToAction("Index");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public async Task<IActionResult> MoveItem(int index, int move)
        {
            var awsService = new AwsService<ICollection<ParentCategory>>(_s3Client, AdSaleConstants.ConfigKey);
            ICollection<ParentCategory> existingCategories = await awsService.GetObject(AdSaleConstants.MediaObjectKey);

            List<ParentCategory> catList = existingCategories.ToList();
            var item = catList[index];
            catList.RemoveAt(index);
            catList.Insert(index + move, item);

            await awsService.SaveObject(catList, AdSaleConstants.MediaObjectKey);

            return RedirectToAction("Index");
        }
    }
}
