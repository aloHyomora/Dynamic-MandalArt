using System;
using Supabase;
using UnityEngine;
using Client = Supabase.Client;
using System.Collections.Generic;
using Newtonsoft.Json;
using Postgrest.Attributes;
using Postgrest.Models; // Postgrest.Models.BaseModels

namespace com.example
{
    [Table("DynamicMandalArt")]
    public class DynamicMandalArt : BaseModel  //
    {
        [PrimaryKey("id")]
        public int id { get; set; }
        
        [Column("goal")]
        public string goal { get; set; }

        [Column("activate_dates")]
        public List<string> activate_dates { get; set; }

        public DynamicMandalArt()
        {
            activate_dates = new List<string>();
        }

        // 날짜 추가 메서드
        public void AddData(string date)
        {
            if(!activate_dates.Contains(date))
                activate_dates.Add(date);
        }
        
        // JSONB 데이터 변환 메서드 (supabase 저장용)
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this.activate_dates);
        }
        
        // JSONB 데이터를 리스트로 변환 (supabase 로드용)
        public static List<string> FromJson(string jsonData)
        {
            return JsonConvert.DeserializeObject<List<string>>(jsonData);
        }

        public override bool Equals(object obj)
        {
            return obj is DynamicMandalArt productInstance &&
                   id == productInstance.id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(id);
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
                Debug.Log($"Id: {product.id}, Dates: ");
            }

        }


        private async void SaveDataWithJsonb(string catego)
        {
            
        }
    }
}