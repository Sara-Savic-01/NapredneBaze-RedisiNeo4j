using Filmofil.Domain.Entities;
using Filmofil.Domain.Entities.Enums;
using Filmofil.Services.Repositories;
using Filmofil.Services.Repositories.Categories;
using System;
using System.Collections.Generic;

namespace Filmofil.Business.CategorieManagment
{
    public class CategorieManager : ICategorieManager
    {
        private readonly IRedisRepository _redisRepository;
        private readonly ICategorieRepository _categorieRepository;

        public CategorieManager(
            IRedisRepository redisRepository,
            ICategorieRepository categorieRepository)
        {
            _redisRepository = redisRepository;
            _categorieRepository = categorieRepository;
        }

        public async void CheckCategoriesInRedis()
        {
            List<Categorie> categories = await _redisRepository
                .GetAsync<List<Categorie>>(RedisKeys.CATEGORIES).ConfigureAwait(false);

            List<Categorie> categoriesToAdd;

            if (categories == null)
            {
                await _redisRepository.SetAsync(RedisKeys.CATEGORIES,
                    TestCategories.AllCategories).ConfigureAwait(false);

                categoriesToAdd = TestCategories.AllCategories;
            }
            else
            {
                List<long> categoriesInRedisIds = categories.ConvertAll(
                    new Converter<Categorie, long>(IdsConverter));

                categoriesToAdd = TestCategories.AllCategories.FindAll(
                    cat=> !categoriesInRedisIds.Contains(cat.Id));

                

                await _redisRepository.SetAsync(RedisKeys.CATEGORIES,
                    categories).ConfigureAwait(false);
            }

            foreach (Categorie cat in categoriesToAdd)
            {
          
                await _categorieRepository.CreateAsync(cat).ConfigureAwait(false);

                
                string key = cat.Title + "-" + RedisKeys.NEW_POSTS;
                await _redisRepository.SetAsync(key, new List<Post>()).ConfigureAwait(false);

              
                key = cat.Title + "-" + RedisKeys.BEST_POSTS;
                await _redisRepository.SetAsync(key, new List<Post>()).ConfigureAwait(false);

                
                key = cat.Title + "-" + RedisKeys.POPULAR_POSTS;
                await _redisRepository.SetAsync(key, new List<Post>()).ConfigureAwait(false);

                key = cat.Title + "-" + RedisKeys.COMMENTS;
                await _redisRepository.SetAsync(key, new List<Comment>()).ConfigureAwait(false);
            }

            
            await _redisRepository.SetAsync(RedisKeys.NEW_POSTS,
                new List<Post>()).ConfigureAwait(false);

            await _redisRepository.SetAsync(RedisKeys.BEST_POSTS,
                new List<Post>()).ConfigureAwait(false);

            await _redisRepository.SetAsync(RedisKeys.POPULAR_POSTS,
                new List<Post>()).ConfigureAwait(false);

            await _redisRepository.SetAsync(RedisKeys.COMMENTS,
                new List<Post>()).ConfigureAwait(false);

            await _redisRepository.SetAsync(RedisKeys.USERS,
                new List<Post>()).ConfigureAwait(false);
        }
        private long IdsConverter(IEntity entity)
        {
            return entity != null ? entity.Id : 0;
        }
    }
}
