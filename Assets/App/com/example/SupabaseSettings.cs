using UnityEngine;

namespace com.example
{
	[CreateAssetMenu(fileName = "Supabase", menuName = "Supabase/Supabase Settings", order = 1)]
	public class SupabaseSettings : ScriptableObject
	{
		public string SupabaseURL = "https://vdurujwnzqxvxjypxycq.supabase.co";
		public string SupabaseAnonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InZkdXJ1anduenF4dnhqeXB4eWNxIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzYwMDQ5MzgsImV4cCI6MjA1MTU4MDkzOH0.C2grETn3CMR1iQqADryP6wxml1s2D613A_bJOjB7p6E";
	}
}
