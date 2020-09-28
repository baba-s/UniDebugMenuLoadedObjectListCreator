using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

namespace Kogane.DebugMenu
{
	/// <summary>
	/// 読み込み済みの UnityEngine.Object の一覧を表示するクラス
	/// </summary>
	public sealed class LoadedObjectListCreator<T> : ListCreatorBase<ActionData> where T : UnityEngine.Object
	{
		//==============================================================================
		// クラス
		//==============================================================================
		private sealed class LoadedData
		{
			public long   Size { get; }
			public string Text { get; }

			public LoadedData( T obj )
			{
				Size = Profiler.GetRuntimeMemorySizeLong( obj );
				var memory = ( Size >> 10 ) / 1024f;
				Text = $"{memory:0.00} MB    {obj.name}";
			}
		}

		//==============================================================================
		// 変数
		//==============================================================================
		private ActionData[] m_list;

		//==============================================================================
		// プロパティ
		//==============================================================================
		public override int Count => m_list.Length;

		//==============================================================================
		// 関数
		//==============================================================================
		/// <summary>
		/// リストの表示に使用するデータを作成します
		/// </summary>
		protected override void DoCreate( ListCreateData data )
		{
			m_list = Resources
					.FindObjectsOfTypeAll<T>()
					.Where( x => ( x.hideFlags & HideFlags.NotEditable ) == 0 )
					.Where( x => ( x.hideFlags & HideFlags.HideAndDontSave ) == 0 )
					.Select( x => new LoadedData( x ) )
					.Where( x => data.IsMatch( x.Text ) )
					.OrderByDescending( x => x.Size )
					.Select( x => new ActionData( x.Text ) )
					.ToArray()
				;

			if ( data.IsReverse )
			{
				Array.Reverse( m_list );
			}
		}

		/// <summary>
		/// 指定されたインデックスの要素の表示に使用するデータを返します
		/// </summary>
		protected override ActionData DoGetElemData( int index )
		{
			return m_list.ElementAtOrDefault( index );
		}
	}
}