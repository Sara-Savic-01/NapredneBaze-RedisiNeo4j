using Filmofil.Domain.Entities;
using System.Collections.Generic;

namespace Filmofil.Business.CategorieManagment
{
    public static class TestCategories
    {
        public static readonly List<Categorie> AllCategories = new List<Categorie>()
        {
            new Categorie()
            {
                Id = 1,
                Title = "Akcija",
                Users = new List<long>(),
                Posts = new List<long>(),
                Messages = new List<long>()
            },
            new Categorie()
            {
                Id = 2,
                Title = "Drama",
                Users = new List<long>(),
                Posts = new List<long>(),
                Messages = new List<long>()
            },
            new Categorie()
            {
                Id = 3,
                Title = "Dokumentarac",
                Users = new List<long>(),
                Posts = new List<long>(),
                Messages = new List<long>()
            },
            new Categorie()
            {
                Id = 4,
                Title = "Fantazija",
                Users = new List<long>(),
                Posts = new List<long>(),
                Messages = new List<long>()
            },
            new Categorie()
            {
                Id = 5,
                Title = "Horor",
                Users = new List<long>(),
                Posts = new List<long>(),
                Messages = new List<long>()
            },
            new Categorie()
            {
                Id = 6,
                Title = "Komedija",
                Users = new List<long>(),
                Posts = new List<long>(),
                Messages = new List<long>()
            },
            new Categorie()
            {
                Id = 7,
                Title = "Romansa",
                Users = new List<long>(),
                Posts = new List<long>(),
                Messages = new List<long>()
            },
            new Categorie()
            {
                Id = 8,
                Title = "Triler",
                Users = new List<long>(),
                Posts = new List<long>(),
                Messages = new List<long>()
            },
            new Categorie()
            {
                Id = 9,
                Title = "Misterija",
                Users = new List<long>(),
                Posts = new List<long>(),
                Messages = new List<long>()
            },
            new Categorie()
            {
                Id = 10,
                Title = "Biografija",
                Users = new List<long>(),
                Posts = new List<long>(),
                Messages = new List<long>()
            }
           
        };
    }
}
