using Blobs;
using Blobs.Interfaces;

using UnityEditor;

using UnityEngine;

[CustomPropertyDrawer( typeof( NamedGroup ) )]
public class NamedGroupPropertyDrawer : PropertyDrawer
{
	public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
	{
		EditorGUI.BeginProperty( position, label, property );
		var originalPosition = position;

		position.height = EditorGUIUtility.singleLineHeight;

		var nameProperty = property.FindPropertyRelative( nameof(NamedGroup.Name) );
		var blobableProperty = property.FindPropertyRelative( nameof(NamedGroup.BlobableSOs) );

		var expadPos = position;
		expadPos.width = 15;
		blobableProperty.isExpanded = EditorGUI.Foldout( expadPos, blobableProperty.isExpanded, "", false );

		var namePropertyPos = position;
		namePropertyPos.x += 20;
		namePropertyPos.width = 160;
		nameProperty.stringValue = EditorGUI.TextField( namePropertyPos, nameProperty.stringValue );

		var minusPos = position;
		minusPos.x += 185;
		minusPos.width = 15;
		var oldEnabled = GUI.enabled;
		GUI.enabled = oldEnabled && blobableProperty.arraySize > 0;
		if ( GUI.Button( minusPos, "-" ) )
		{
			blobableProperty.arraySize--;
		}
		GUI.enabled = oldEnabled;

		var sizePos = position;
		sizePos.x += 200;
		sizePos.width = 45;
		blobableProperty.arraySize = EditorGUI.IntField( sizePos, blobableProperty.arraySize );

		var plusPos = position;
		plusPos.x += 255;
		plusPos.width = 15;
		if ( GUI.Button( plusPos, "+" ) )
		{
			blobableProperty.arraySize++;
		}

		position.y += EditorGUIUtility.singleLineHeight;

		if ( blobableProperty.isExpanded )
		{
			for ( int i = 0; i < blobableProperty.arraySize; i++ )
			{
				EditorGUI.PropertyField( position, blobableProperty.GetArrayElementAtIndex( i ), true );
				position.y += EditorGUIUtility.singleLineHeight;
			}
		}

		//EditorGUI.PropertyField( position, blobableProperty, true );
		EditorGUI.EndProperty();

		if ( Event.current.type == EventType.DragUpdated && originalPosition.Contains( Event.current.mousePosition ) )
		{
			DragAndDrop.visualMode = DragAndDropVisualMode.Link;
			Event.current.Use();
		}
		else if ( Event.current.type == EventType.DragPerform && originalPosition.Contains( Event.current.mousePosition ) )
		{
			foreach ( Object draggedObject in DragAndDrop.objectReferences )
			{
				if ( draggedObject is IBlobableSO )
				{
					if ( blobableProperty.arraySize > 0 && blobableProperty.GetArrayElementAtIndex( 0 ) != null )
					{
						if ( blobableProperty.GetArrayElementAtIndex( 0 ).objectReferenceValue.GetType() == draggedObject.GetType() )
						{
							blobableProperty.arraySize++;
							blobableProperty.GetArrayElementAtIndex( blobableProperty.arraySize - 1 ).objectReferenceValue = draggedObject;
						}
					}
					else
					{
						blobableProperty.arraySize++;
						blobableProperty.GetArrayElementAtIndex( blobableProperty.arraySize - 1 ).objectReferenceValue = draggedObject;
					}
				}
			}
			DragAndDrop.AcceptDrag();
		}
	}

	public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
	{
		float totalHeight = 0;

		var nameHeight = EditorGUIUtility.singleLineHeight;
		totalHeight += nameHeight;

		var blobableProperty = property.FindPropertyRelative( nameof(NamedGroup.BlobableSOs) );
		float blobableHeight = EditorGUIUtility.singleLineHeight; //Label
		if ( blobableProperty.isExpanded )
		{
			blobableHeight += EditorGUIUtility.singleLineHeight * blobableProperty.arraySize;
		}
		totalHeight += blobableHeight;

		return totalHeight;
	}
}