using System;
using Supabase;
using UnityEngine;
using Client = Supabase.Client;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Postgrest.Attributes;
using Postgrest.Models;
using TMPro;
using UnityEngine.UI; // Postgrest.Models.BaseModels

namespace com.example
{
    [Table("DynamicMandalArt")]
    public class DynamicMandalArt : BaseModel  //
    {
        [PrimaryKey("id")]
        public int id { get; set; }
        
        [Column("index")]
        public int index { get; set; }
        
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
        public static SupabaseHandler Instance;
        public SupabaseSettings SupabaseSettings = null!;
        private Client client;
        public GameObject mandalArtGrid;
        private async void Start()
        {
            if (Instance == null) Instance = this;
            var options = new SupabaseOptions
            {
                AutoConnectRealtime = true
            };

            var supabase = new Supabase.Client(SupabaseSettings.SupabaseURL, SupabaseSettings.SupabaseAnonKey, options);
            await supabase.InitializeAsync();
            
            // 데이터 로드
            await LoadData();
        }

        private List<DynamicMandalArt> GetIdGoalFromUnity()
        {
            Button[] buttons = mandalArtGrid.GetComponentsInChildren<Button>();

            List<DynamicMandalArt> initialData = new List<DynamicMandalArt>();

            for (int i = 0; i < buttons.Length; i++)
            {
                DynamicMandalArt data = new DynamicMandalArt();

                int index = 0;
                if (buttons[i].name.StartsWith("Button"))
                {
                    string[] parts = buttons[i].name.Substring(7).Split('_');

                    if (parts.Length == 2 && int.TryParse(parts[0], out int row) && int.TryParse(parts[1], out int col))
                    {
                        index = row * 10 + col;
                    }
                }

                data.index = index;
                data.goal = buttons[i].GetComponentInChildren<TextMeshProUGUI>().text.Replace(" ", "").Replace("\n", "");

                Debug.Log($"id: {data.id}, goal: {data.goal}");
                initialData.Add(data);
            }

            return initialData;
        }
        public async void SaveDataWithJsonb(int index, string newDate)
        {
            try
            {
                var options = new SupabaseOptions
                {
                    AutoConnectRealtime = true
                };

                var supabase = new Supabase.Client(SupabaseSettings.SupabaseURL, SupabaseSettings.SupabaseAnonKey, options);
                
                // 특정 ID의 데이터 가져오기
                var existingData = await supabase.From<DynamicMandalArt>().Where(x => x.index == index).Single();

                if (existingData != null)
                {
                    // 날짜 추가
                    existingData.AddData(newDate);
                    
                    // 데이터 업데이트
                    await supabase.From<DynamicMandalArt>().Update(existingData);
                    Debug.Log($"Updated DynamicMandalArt with ID: {index}, Added Date: {newDate}");
                }
                else
                {
                    Debug.LogWarning($"No DynamicMandalArt found with ID: {index}. Creating new entry.");

                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error in SaveDataWithJsonb: {e.Message}");
                throw;
            }
        }
        
        // 데이터 로드 메서드
        private async Task LoadData()
        {
            try
            {
                var supabase = new Supabase.Client(SupabaseSettings.SupabaseURL, SupabaseSettings.SupabaseAnonKey, new SupabaseOptions { AutoConnectRealtime = true });

                // 모든 데이터 가져오기
                var result = await supabase.From<DynamicMandalArt>().Get();

                List<DynamicMandalArt> resultModels = result.Models;
                Debug.Log($"Loaded {resultModels.Count} entries from DynamicMandalArt table.");
                
                // 데이터 없으면 데이터 생성함. (초기 1회)
                if (resultModels.Count == 0)
                {
                    await InsertInitialData();
                }

                foreach (var entry in resultModels)
                {
                    Debug.Log($"Index: {entry.index}, Goal: {entry.goal}, Dates: {string.Join(", ", entry.activate_dates)}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in LoadData: {ex.Message}");
            }
        }
        
        private async Task InsertInitialData()
        {
            try
            {
                var supabase = new Supabase.Client(SupabaseSettings.SupabaseURL, SupabaseSettings.SupabaseAnonKey, new SupabaseOptions { AutoConnectRealtime = true });

                // 초기 데이터 리스트
                List<DynamicMandalArt> initialData = GetIdGoalFromUnity();

                foreach (var data in initialData)
                {
                    await supabase.From<DynamicMandalArt>().Insert(data);
                    Debug.Log($"Inserted Data: ID = {data.id}, Goal = {data.goal}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error inserting initial data: {ex.Message}");
            }
        }
    }
}