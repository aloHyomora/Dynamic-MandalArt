using System;
using Supabase;
using UnityEngine;
using Client = Supabase.Client;
using System.Collections.Generic;
using Postgrest.Attributes;
using Postgrest.Models; // Postgrest.Models.BaseModels

namespace com.example
{
    [Table("DynamicMandalArt")]
    public class DynamicMandalArt : BaseModel  //
    {
        [PrimaryKey("goal_id")]
        public int goal_id { get; set; }
        
        // [Column("activate_dates")]
        

        public override bool Equals(object obj)
        {
            return obj is DynamicMandalArt productInstance &&
                   goal_id == productInstance.goal_id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(goal_id);
        }

    }

    public class SupabaseHandler : MonoBehaviour
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
            
            var result = await supabase.From<DynamicMandalArt>().Get();
            Debug.Log(result +" !");

            List<DynamicMandalArt> resultModels = result.Models;
            Debug.Log($"Product count: {resultModels.Count}");

            foreach (var product in resultModels)
            {
                Debug.Log($"Id: {product.goal_id}, Dates: ");
            }

        }
    }
}