﻿using System;
using System.Linq;

using BetterCms.Core.Exceptions.DataTier;
using BetterCms.Core.Mvc.Commands;

using BetterCms.Module.Pages.Models;
using BetterCms.Module.Pages.Services;
using BetterCms.Module.Pages.ViewModels.Widgets;
using BetterCms.Module.Root.Models;
using BetterCms.Module.Root.Mvc;
using BetterCms.Module.Root.Services;
using BetterCms.Module.Root.ViewModels.Option;

namespace BetterCms.Module.Pages.Command.Widget.GetServerControlWidgetForEdit
{
    /// <summary>
    /// A command to get widget by id for editing.
    /// </summary>
    public class GetServerControlWidgetForEditCommand : CommandBase, ICommand<Guid?, EditServerControlWidgetViewModel>
    {
        /// <summary>
        /// The category service
        /// </summary>
        private readonly ICategoryService categoryService;

        /// <summary>
        /// The content service
        /// </summary>
        private readonly IContentService contentService;

        /// <summary>
        /// The options service
        /// </summary>
        private readonly IOptionService optionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetServerControlWidgetForEditCommand" /> class.
        /// </summary>
        /// <param name="categoryService">The category service.</param>
        /// <param name="contentService">The content service.</param>
        /// <param name="optionService">The option service.</param>
        public GetServerControlWidgetForEditCommand(ICategoryService categoryService, IContentService contentService, IOptionService optionService)
        {
            this.contentService = contentService;
            this.categoryService = categoryService;
            this.optionService = optionService;
        }

        /// <summary>
        /// Executes this command.
        /// </summary>
        /// <param name="widgetId">The request.</param>
        /// <returns>
        /// Executed command result.
        /// </returns>
        public EditServerControlWidgetViewModel Execute(Guid? widgetId)
        {            
            EditServerControlWidgetViewModel model = null;

            if (widgetId.HasValue && widgetId.Value != Guid.Empty)
            {
                var serverControlWidget = contentService.GetContentForEdit(widgetId.Value) as ServerControlWidget;

                if (serverControlWidget != null)
                {
                    model = new EditServerControlWidgetViewModel {
                                                                     Id = serverControlWidget.Id,
                                                                     Version = serverControlWidget.Version,
                                                                     Name = serverControlWidget.Name,
                                                                     Url = serverControlWidget.Url,
                                                                     PreviewImageUrl = serverControlWidget.PreviewUrl,
                                                                     CurrentStatus = serverControlWidget.Status,
                                                                     HasPublishedContent = serverControlWidget.Original != null,
                                                                     WidgetType = WidgetType.ServerControl                                                                     
                                                                 };

                    model.Options = serverControlWidget.ContentOptions.Distinct()
                        .Select(
                            f => 
                                new OptionViewModel
                                 {
                                     Type = f.Type,
                                     OptionDefaultValue = optionService.ClearFixValueForEdit(f.Type, f.DefaultValue),
                                     OptionKey = f.Key,
                                     CanDeleteOption = f.IsDeletable,
                                     CustomOption = f.CustomOption != null
                                        ? new CustomOptionViewModel
                                          {
                                              Identifier = f.CustomOption.Identifier,
                                              Title = f.CustomOption.Title,
                                              Id = f.CustomOption.Id
                                          }
                                        : null
                                 })
                        .OrderBy(o => o.OptionKey)
                        .ToList();
                    optionService.SetCustomOptionValueTitles(model.Options);
                }

                if (model == null)
                {
                    throw new EntityNotFoundException(typeof(ServerControlWidget), widgetId.Value);
                }
            }
            else
            {
                model = new EditServerControlWidgetViewModel();
            }

            model.Categories = categoryService.GetSelectedCategories<Root.Models.Widget, WidgetCategory>(widgetId).ToList();
            model.CustomOptions = optionService.GetCustomOptions();
            model.CategoriesFilterKey = Root.Models.Widget.CategorizableItemKeyForWidgets;
            
            return model;
        }
    }
}