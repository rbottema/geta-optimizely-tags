﻿using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Data;
using EPiServer.Data.Dynamic;
using EPiServer.ServiceLocation;
using Geta.Tags.Interfaces;
using Geta.Tags.Models;

namespace Geta.Tags.Implementations
{
    [ServiceConfiguration(typeof(ITagRepository))]
    public class TagRepository : ITagRepository
    {
        private static DynamicDataStore TagStore => typeof(Tag).GetOrCreateStore();

        public Tag GetTagById(Identity id)
        {
            return TagStore.Load<Tag>(id);
        }

        public Tag GetTagByName(string name)
        {
            return TagStore.Find<Tag>("Name", name).FirstOrDefault();
        }

        public IEnumerable<Tag> GetTagsByContent(Guid contentGuid)
        {
            return GetAllTags().Where(t => t.PermanentLinks.Contains(contentGuid));
        }

        public IQueryable<Tag> GetAllTags()
        {
            return TagStore.Items<Tag>();
        }

        public Identity Save(Tag tag)
        {
            if (string.IsNullOrEmpty(tag?.Name))
            {
                return null;
            }

            var existingTag = GetTagByName(tag.Name);
            return existingTag != null
                ? TagStore.Save(tag, existingTag.GetIdentity())
                : TagStore.Save(tag);
        }

        public void Delete(Tag tag)
        {
            TagStore.Delete(tag);
        }
    }
}