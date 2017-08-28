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
    public class CategoryController : Controller
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IMapper _mapper;

        public CategoryController(IAmazonS3 s3Client, IMapper mapper)
        {
            _s3Client = s3Client;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var awsService = new AwsService<ICollection<ParentCategory>>(_s3Client, AdSaleConstants.ConfigKey);

            var result = await awsService.GetObject(AdSaleConstants.CategoriesKey);

            return View(result);
        }

        [HttpGet]
        public IActionResult Create(string parentId = null)
        {
            // make sure the visible flag is true by default
            var model = new CategoryModel { IsActive = true, ParentId = parentId };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryModel model)
        {
            if (ModelState.IsValid)
            {
                var awsService = new AwsService<ICollection<ParentCategory>>(_s3Client, AdSaleConstants.ConfigKey);
                var existingCategories = await awsService.GetObject(AdSaleConstants.CategoriesKey) ?? new List<ParentCategory>();

                // is parent
                if (string.IsNullOrEmpty(model.ParentId))
                {
                    var newUniqueId = IdGenerator.Make(existingCategories.Select(x => x.Id).ToList());
                    var adSaleCategory = new ParentCategory
                    {
                        Id = newUniqueId,
                        Name = model.Name,
                        IsActive = model.IsActive
                    };

                    existingCategories.Add(adSaleCategory);
                }
                else
                {
                    var parent = existingCategories.First(x => x.Id == model.ParentId);
                    if (parent.Subcategories == null)
                    {
                        parent.Subcategories = new List<Category>();
                    }

                    var newUniqueId = IdGenerator.Make(parent.Subcategories.Select(x => x.Id).ToList(), parent.Id);

                    parent.Subcategories.Add(new Category
                    {
                        Id = newUniqueId,
                        Name = model.Name,
                        IsActive = model.IsActive
                    });
                }

                await awsService.SaveObject(existingCategories, AdSaleConstants.CategoriesKey);

                return RedirectToAction("Index");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var awsService = new AwsService<ICollection<ParentCategory>>(_s3Client, AdSaleConstants.ConfigKey);
            var existingCategories = await awsService.GetObject(AdSaleConstants.CategoriesKey);
            CategoryModel model;

            if (!id.Contains("-"))
            {
                // it's a parent
                var existingCategory = existingCategories.First(x => x.Id == id);
                model = _mapper.Map<CategoryModel>(existingCategory);
            }
            else
            {
                // it's a child
                var parentId = id.Substring(0, id.IndexOf('-'));
                var subCategoryToEdit = existingCategories.First(x => x.Id == parentId).Subcategories.First(s => s.Id == id);
                model = _mapper.Map<CategoryModel>(subCategoryToEdit);
                model.ParentId = parentId;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryModel model)
        {
            if (ModelState.IsValid)
            {
                var awsService = new AwsService<ICollection<ParentCategory>>(_s3Client, AdSaleConstants.ConfigKey);
                var existingCategories = await awsService.GetObject(AdSaleConstants.CategoriesKey);

                if (string.IsNullOrEmpty(model.ParentId))
                {
                    var adSaleCategoryParent = existingCategories.First(x => x.Id == model.Id);
                    adSaleCategoryParent.IsActive = model.IsActive;
                    adSaleCategoryParent.Name = model.Name;
                }
                else
                {
                    // it's a child
                    var parentId = model.Id.Substring(0, model.Id.IndexOf('-'));
                    Category subcategoryToEdit = existingCategories.First(x => x.Id == parentId).Subcategories.First(s => s.Id == model.Id);
                    subcategoryToEdit.IsActive = model.IsActive;
                    subcategoryToEdit.Name = model.Name;
                }

                await awsService.SaveObject(existingCategories, AdSaleConstants.CategoriesKey);

                return RedirectToAction("Index");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// Delete a category (and subcategories) from categories.json and appends the deleted category/categories in the file categories-deleted.json
        /// </summary>
        /// <param name="id">The id of the category to delete</param>
        /// <returns>A list of active and inactive categories, excluding the deleted ones</returns>
        public async Task<IActionResult> Delete(string id)
        {
            var awsService = new AwsService<ICollection<ParentCategory>>(_s3Client, AdSaleConstants.ConfigKey);
            var existingCategories = await awsService.GetObject(AdSaleConstants.CategoriesKey);
            var adSaleCategoriesToDelete = new List<Category>();

            if (!id.Contains('-'))
            {
                // it's a parent
                var categoryToDelete = existingCategories.First(x => x.Id == id);
                existingCategories.Remove(categoryToDelete);

                adSaleCategoriesToDelete.Add(new Category
                {
                    Id = categoryToDelete.Id,
                    Name = categoryToDelete.Name,
                    IsActive = categoryToDelete.IsActive
                });
                if (categoryToDelete.Subcategories != null)
                {
                    foreach (var cat in categoryToDelete.Subcategories)
                    {
                        adSaleCategoriesToDelete.Add(cat);
                    }
                }
            }
            else
            {
                // it's a child
                var parentId = id.Substring(0, id.IndexOf('-'));
                var adSaleCategoryParent = existingCategories.First(x => x.Id == parentId);

                Category subcategoryToDelete = adSaleCategoryParent.Subcategories.First(s => s.Id == id);
                adSaleCategoryParent.Subcategories.Remove(subcategoryToDelete);

                adSaleCategoriesToDelete.Add(subcategoryToDelete);
            }

            await awsService.SaveObject(existingCategories, AdSaleConstants.CategoriesKey);

            // append the just-deleted categories to the list of already deleted categories
            var awsService2 = new AwsService<List<Category>>(_s3Client, AdSaleConstants.ConfigKey);
            var existingDeletedCategories = await awsService2.GetObject(AdSaleConstants.CategoriesDeletedKey) ?? new List<Category>();
            existingDeletedCategories.AddRange(adSaleCategoriesToDelete);
            await awsService2.SaveObject(existingDeletedCategories, AdSaleConstants.CategoriesDeletedKey);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> MoveCategoryItem(int index, int move, string parentId = null)
        {
            var awsService = new AwsService<ICollection<ParentCategory>>(_s3Client, AdSaleConstants.ConfigKey);
            ICollection<ParentCategory> existingCategories = await awsService.GetObject(AdSaleConstants.CategoriesKey);

            List<ParentCategory> catList = existingCategories.ToList();
            if (string.IsNullOrEmpty(parentId))
            {
                var item = catList[index];
                catList.RemoveAt(index);
                catList.Insert(index + move, item);
            }
            else
            {
                var parentCategory = catList.FirstOrDefault(x => x.Id == parentId);
                var subCategories = parentCategory.Subcategories.ToList();
                var item = subCategories[index];
                subCategories.RemoveAt(index);
                subCategories.Insert(index + move, item);
                parentCategory.Subcategories = subCategories;
            }

            await awsService.SaveObject(catList, AdSaleConstants.CategoriesKey);

            return RedirectToAction("Index");
        }
    }
}