﻿using System;
using System.Collections.Generic;

namespace BetterCms.Module.Pages.Api.Dto
{
    public class CreateServerControlWidgetRequest
    {
        public string Name { get; set; }

        public string WidgetPath { get; set; }

        public Guid? CategoryId { get; set; }

        public string PreviewUrl { get; set; }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public IList<ContentOptionDto> Options { get; set; }
    }
}