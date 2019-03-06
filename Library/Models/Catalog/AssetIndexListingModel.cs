﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models.Catalog
{
    public class AssetIndexListingModel
    {
        //create POCO Class

        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public string AuthorOrDirector { get; set; }
        public string Type { get; set; }
        public string DewyCallNumber { get; set; }
        public string NumberOfCopies { get; set; }

        public int StatusId { get; set; }
        public int LocationId { get; set; }


    }
}