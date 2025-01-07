using System;
using Supabase;
using UnityEngine;
using Client = Supabase.Client;
using System.Collections.Generic;
using Postgrest.Attributes;
using Postgrest.Models; // Postgrest.Models.BaseModels

namespace com.example
{
    [Table("product")]
    public class product : BaseModel  //
    {
        [PrimaryKey("id")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        
        [Column("model_code")]
        public string ModelCode { get; set; }

        public override bool Equals(object obj)
        {
            return obj is product productInstance &&
                    Id == productInstance.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

    }

    public class SupabaseGet : MonoBehaviour
    {
        public SupabaseSettings SupabaseSettings = null!;
        private Client client;

        private async void Start()
        {
            var options = new SupabaseOptions
            {
                AutoConnectRealtime = true
            };

            var supabase = new Supabase.Client(SupabaseSettings.SupabaseURL, SupabaseSettings.SupabaseAnonKey, options);
            await supabase.InitializeAsync();
            
            var result = await supabase.From<product>().Get();
            Debug.Log(result +" !");

            List<product> products = result.Models;
            Debug.Log($"Product count: {products.Count}");

            foreach (var product in products)
            {
                Debug.Log($"Id: {product.Id}, Name: {product.Name}, Model_Code: {product.ModelCode}");
            }

        }
    }
}