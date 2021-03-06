﻿using System;
using System.Collections.Generic;
using System.Linq;

using BetterCms.Core.DataAccess;
using BetterCms.Core.DataAccess.DataContext;
using BetterCms.Core.Exceptions.DataTier;
using BetterCms.Events;
using BetterCms.Module.Root.Models;
using BetterCms.Module.Root.Mvc;
using BetterCms.Module.Root.Mvc.Grids.Extensions;
using BetterCms.Module.Root.Services.Categories.Nodes;

using NHibernate.Criterion;
using NHibernate.Linq;

namespace BetterCms.Module.Root.Services.Categories.Tree
{
    public class DefaultCategoryTreeService : ICategoryTreeService
    {
        private IRepository Repository;

        private IUnitOfWork UnitOfWork;

        private ICategoryNodeService CategoryNodeService;

        public DefaultCategoryTreeService(IRepository repository, IUnitOfWork unitOfWork, ICategoryNodeService categoryNodeService)
        {
            Repository = repository;
            UnitOfWork = unitOfWork;
            CategoryNodeService = categoryNodeService;
        }

        public GetCategoryTreeResponse Get(GetCategoryTreeRequest request)
        {
            var response = new GetCategoryTreeResponse();
            CategoryTree alias = null;

            var query = Repository.AsQueryOver(() => alias).Where(() => !alias.IsDeleted);

            if (!string.IsNullOrWhiteSpace(request.SearchQuery))
            {
                var searchQuery = string.Format("%{0}%", request.SearchQuery);
                query =
                    query.Where(Restrictions.Disjunction().Add(Restrictions.InsensitiveLike(NHibernate.Criterion.Projections.Property(() => alias.Title), searchQuery)));
            }

            if (request.WithOrder)
            {
                query = query.AddOrder(request);
            }

            if (request.WithPaging)
            {
                query = query.AddPaging(request);
            }

            response.Items = query.AddSortingAndPaging(request).Future<CategoryTree>();
            response.TotalCount = query.ToRowCountFutureValue().Value;
            return response;
        }

        public CategoryTree Save(SaveCategoryTreeRequest request)
        {
            IList<Category> createdCategories = new List<Category>();

            IList<Category> updatedCategories = new List<Category>();

            IList<Category> deletedCategories = new List<Category>();

            var createNew = request.Id.HasDefaultValue();

            var selectedCategorizableItemsFuture = Repository.AsQueryable<CategoryTreeCategorizableItem>()
                .Where(i => i.CategoryTree.Id == request.Id)
                .ToFuture();

            IEnumerable<Category> existingCategories;
            CategoryTree categoryTree;
            if (createNew)
            {
                existingCategories = new List<Category>();
                categoryTree = new CategoryTree();
            }
            else
            {
                existingCategories = Repository.AsQueryable<Category>().Where(node => node.CategoryTree.Id == request.Id).ToFuture();
                categoryTree = Repository.AsQueryable<CategoryTree>().Where(s => s.Id == request.Id).ToFuture().First();
            }

            List<CategoryTreeCategorizableItem> itemsToRemove = null;
            List<Guid> itemsToAdd = null;
            if (request.UseForCategorizableItems != null)
            {
                var selectedItems = selectedCategorizableItemsFuture.ToList();
                itemsToRemove = selectedItems.Where(s => !request.UseForCategorizableItems.Exists(c => c == s.CategorizableItem.Id)).ToList();
                itemsToAdd = request.UseForCategorizableItems.Where(id => !selectedItems.Exists(s => s.CategorizableItem.Id == id)).ToList();
            }


            UnitOfWork.BeginTransaction();

            categoryTree.Title = request.Title;
            categoryTree.Version = request.Version;
            categoryTree.Macro = request.Macro;

            Repository.Save(categoryTree);

            if (itemsToRemove != null)
            {
                itemsToRemove.ForEach(Repository.Delete);
                itemsToAdd.ForEach(
                    id =>
                    Repository.Save(new CategoryTreeCategorizableItem { CategoryTree = categoryTree, CategorizableItem = Repository.AsProxy<CategorizableItem>(id) }));
            }

            var existingCategoryList = existingCategories.ToList();
            SaveCategoryTreeNodes(categoryTree, request.RootNodes, null, existingCategoryList, createdCategories, updatedCategories);

            foreach (var category in existingCategoryList)
            {
                Repository.Delete(category);
                deletedCategories.Add(category);
            }

            UnitOfWork.Commit();

            foreach (var category in createdCategories)
            {
                RootEvents.Instance.OnCategoryCreated(category);
            }

            foreach (var category in updatedCategories)
            {
                RootEvents.Instance.OnCategoryUpdated(category);
            }

            foreach (var category in deletedCategories)
            {
                RootEvents.Instance.OnCategoryDeleted(category);
            }

            if (createNew)
            {
                RootEvents.Instance.OnCategoryTreeCreated(categoryTree);
            }
            else
            {
                RootEvents.Instance.OnCategoryTreeUpdated(categoryTree);
            }

            return categoryTree;
        }

        public bool Delete(DeleteCategoryTreeRequest request)
        {
            var categoryTree = Repository
                .AsQueryable<CategoryTree>()
                .Where(map => map.Id == request.Id)
                // TODO:                .FetchMany(map => map.AccessRules)
                .Distinct()
                .ToList()
                .First();

            // TODO:            // Security.
            //            if (cmsConfiguration.Security.AccessControlEnabled)
            //            {
            //                var roles = new[] { RootModuleConstants.UserRoles.EditContent };
            //                accessControlService.DemandAccess(sitemap, currentUser, AccessLevel.ReadWrite, roles);
            //            }

            // Concurrency.
            if (request.Version > 0 && categoryTree.Version != request.Version)
            {
                throw new ConcurrentDataException(categoryTree);
            }

            UnitOfWork.BeginTransaction();

            // TODO:
            //            if (sitemap.AccessRules != null)
            //            {
            //                var rules = sitemap.AccessRules.ToList();
            //                rules.ForEach(sitemap.RemoveRule);
            //            }

            Repository.Delete(categoryTree);

            UnitOfWork.Commit();

            RootEvents.Instance.OnCategoryTreeDeleted(categoryTree);
            // Events.
            // TODO:            Events.SitemapEvents.Instance.OnSitemapDeleted(categoryTree);
            return true;
        }


        private void SaveCategoryTreeNodes(CategoryTree categoryTree, 
            IEnumerable<CategoryNodeModel> categories, 
            Category parentCategory, 
            ICollection<Category> existingCategories, 
            IList<Category> createdCategories,
            IList<Category> updatedCategories)
        {
            if (categories == null)
            {
                return;
            }

            foreach (var categoryNodeModel in categories)
            {
                bool updatedInDB;
                var category = CategoryNodeService.SaveCategory(out updatedInDB, categoryTree, categoryNodeModel, parentCategory, existingCategories);

                if (categoryNodeModel.Id.HasDefaultValue() && updatedInDB)
                {
                    createdCategories.Add(category);
                }
                else if (updatedInDB)
                {
                    updatedCategories.Add(category);
                }
                var existingCategory = existingCategories.FirstOrDefault(c => c.Id == categoryNodeModel.Id);
                if (existingCategory != null)
                {
                    existingCategories.Remove(existingCategory);
                }

                SaveCategoryTreeNodes(categoryTree, categoryNodeModel.ChildNodes, category, existingCategories, createdCategories, updatedCategories);
            }
        }
    }
}