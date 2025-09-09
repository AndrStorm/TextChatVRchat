
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

using TMPro;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
#endif


[ExecuteAlways]
[RequireComponent( typeof( CanvasGroup ) )]
public class UdonUltimateChatBox : UdonSharpBehaviour
{

    /*
    #region Fields

    // INTERNAL //
		
    
    /*[Serializable] public class ChatInformation
         		{
         			public UdonUltimateChatBox chatBox;
         
         			/// <summary>
         			/// Returns the current state of this chat being visible.
         			/// </summary>
         			public bool IsVisible
         			{
         				get;
         				private set;
         			}
         			public TextMeshProUGUI chatText;
         			/// <summary>
         			/// The exact username that was provided when registering a chat. If the provided username contained a number, this string includes that number for reference.
         			/// </summary>
         			public string Username { get; set; }
         			/// <summary>
         			/// The display username. If the username provided when registering a chat contained a number, then this string will be just the name portion, excluding the associated number.
         			/// </summary>
         			public string DisplayUsername { get; set; }
         			/// <summary>
         			/// The exact string value provided to the chat box for the message.
         			/// </summary>
         			public string Message { get; set; }
         			/// <summary>
         			/// The modified string value with all the users options applied to it so that it looks correct.
         			/// </summary>
         			public string DisplayMessage { get; set; }
         			public Rect usernameRect = new Rect();
         			public float contentSpace = 0.0f;
         			public Vector2 anchoredPosition = Vector2.zero;
         			public float usernameWidth = 0.0f;
         			public float lineHeight = 0.0f;
         			public int lineCount = 0;
         			public ChatStyle chatBoxStyle;
         
         
         			/// <summary>
         			/// [INTERNAL] Updates the text component of this chat so that it can be display.
         			/// </summary>
         			public void UpdateText ()
         			{
         				// If the assigned text is null, or the provided style is null, return.
         				if( chatText == null || chatBoxStyle == null )
         					return;
         
         				// Update the text component to display the chat.
         				chatText.text = chatBoxStyle.FormatMessage( DisplayUsername, DisplayMessage );
         			}
         
         			/// <summary>
         			/// [INTERNAL] Updates the visibility of this chat.
         			/// </summary>
         			public void UpdateVisibility ()
         			{
         				// Check the top position of the text and if the chat box contains the position then set IsVisible to true.
         				if( chatBox.chatBoxVisiblityRect.Contains( anchoredPosition * -1 ) )
         					IsVisible = true;
         				// Else check the position of the text + the content space, and if it is within the chat box then set IsVisible to true.
         				else if( chatBox.chatBoxVisiblityRect.Contains( ( anchoredPosition * -1 ) + new Vector2( 0, contentSpace - 0.01f ) ) )
         					IsVisible = true;
         				// Else if the content space is actually larger than the visible bounds of the chat box... 
         				else if( contentSpace > chatBox.visibleChatBoundingBox.sizeDelta.y )
         				{
         					// Calculate difference of the visible bounds of the chat box and the space of this content.
         					float differenceModifier = chatBox.visibleChatBoundingBox.sizeDelta.y / contentSpace;
         
         					// Loop for adjusting the checks for the content space to see if this chat is indeed visible...
         					for( float mod = 0.0f; mod < 1.0f; mod += differenceModifier )
         					{
         						// If the chat box contains the content spaced * mod above, then set IsVisible to true and return.
         						if( chatBox.chatBoxVisiblityRect.Contains( ( anchoredPosition * -1 ) + new Vector2( 0, contentSpace * mod ) ) )
         						{
         							IsVisible = true;
         							return;
         						}
         					}
         
         					// Else set IsVisible to false.
         					IsVisible = false;
         				}
         				// Else none of the other checks were true so the chat isn't visible. Set IsVisible to false.
         				else
         					IsVisible = false;
         			}
         
         			/// <summary>
         			/// Removes this chat information from the chat box.
         			/// </summary>
         			public void RemoveChat ()
         			{
         				// If the chat text is assigned, then send it to the pool to be reused.
         				if( chatText != null )
         					chatBox.SendTextToPool( chatText );
         
         				// Remove this chat information from the list.
         				chatBox.ChatInformations.Remove( this );
         
         				// Set isDirty to true so the chat box knows to update the positioning of everything.
         				chatBox.isDirty = true;
         			}
         		}
         		#1#
                
                
		/// <summary>
		/// The list of all the registered chat informations.
		/// </summary>
		public List<ChatInformation> ChatInformations { get; private set; } = new List<ChatInformation>();
		/// <summary>
		/// The list of all the text objects that have been created to display the registered chat information.
		/// </summary>
		public List<TextMeshProUGUI> AllTextObjects { get; private set; } = new List<TextMeshProUGUI>();
		List<TextMeshProUGUI> UnusedTextPool = new List<TextMeshProUGUI>();
		/// <summary>
		/// The parent Canvas that this chat box is placed inside.
		/// </summary>
		public Canvas ParentCanvas { get; private set; }
		RectTransform parentCanvasRectTrans;
		Vector3 parentCanvasScale = Vector3.one;
		Vector2 parentCanvasSize = Vector2.zero;
		/// <summary>
		/// The current state of the chat box being interactable. Setting this value to <see langword="false"/>will not allow input to be processed on the chat box.
		/// </summary>
		public bool Interactable { get; set; } = true;
		/// <summary>
		/// The current state of this Ultimate Chat Box being enabled or disabled.
		/// </summary>
		public bool IsEnabled { get; private set; }
		/// <summary>
		/// Returns the state of the input being on the chat box or not.
		/// </summary>
		public bool InputOnChatBox { get; private set; }
		/// <summary>
		/// The line height of the TextObject to use for chat box navigation and chat spacing.
		/// </summary>
		public float LineHeight { get; private set; }
		float totalContentSpace = 0.0f;
		bool isDraggingScrollHandle = false;
		/// <summary>
		/// The stored position of the input for calculating on the chat box.
		/// </summary>
		public Vector3 InputPosition { get; private set; }
		Vector3 previousInputPosition = Vector2.zero;
		/// <summary>
		/// The state of the input being down on this frame for calculations.
		/// </summary>
		public bool GetButtonDown { get; private set; }
		/// <summary>
		/// The current state of the input being pressed.
		/// </summary>
		public bool GetButton { get; private set; }
		float scrollValue = 0.0f;
		/// <summary>
		/// Set the current scroll value to apply to the chat box.
		/// </summary>
		public float ScrollValue
		{
			set
			{
				scrollValue = value;
			}
		}
		/// <summary>
		/// The total size that the chat box occupies on the screen. This value includes the input field if that is used.
		/// </summary>
		public Vector2 TotalChatBoxSize { get; private set; }
		int inputFieldStringPosition = -1;
		public bool isDirty = false;
		[SerializeField] [HideInInspector]
		private CanvasGroup chatBoxCanvasGroup;
		// Touch Input //
		[SerializeField] [Tooltip( "Should the chat box calculate any touch input on it?" )]
		private bool allowTouchInput = false;
		bool isDraggingWithTouch = false;
		int currentTouchId = -1;
		// Custom Input //
		bool customInputRecieved = false;
		Vector2 customScreenPosition = Vector2.zero;
		bool customGetButtonDown = false, customGetButton = false;

		// CHAT BOX POSITION //
		RectTransform baseTransform;
		/// <summary>
		/// The base transform of the Ultimate Chat Box.
		/// </summary>
		public RectTransform BaseTransform
		{
			get
			{
				if( baseTransform == null )
					baseTransform = GetComponent<RectTransform>();

				return baseTransform;
			}
		}
		[SerializeField] [Tooltip( "The ratio of the chat box." )]
		private Vector2 chatBoxSizeRatio = new Vector2( 1.0f, 0.5f );
		[SerializeField] [Tooltip( "The overall size of the chat box." )]
		private float chatBoxSize = 5.0f;
		[SerializeField] [Tooltip( "The position of the chat box on the screen. These values are calculated as percentages, so they are divided by 100 and calculated off the canvas size so that it will be consistent across all screen sizes." )]
		private Vector2 chatBoxPosition = new Vector2( 5.0f, 10.0f );
		[SerializeField] [Tooltip( "The visible bounding box for the chat in the chat box." )]
		public RectTransform visibleChatBoundingBox;
		/// <summary>
		/// Returns the RectTransform that is used as the visible mask for the chat box.
		/// </summary>
		public RectTransform VisibleChatBoundingBox
		{
			get
			{
				// If the boundingBox is null, inform the user on how to fix the issue. This should never happen though.
				if( visibleChatBoundingBox == null )
					Debug.LogError( FormatDebug( "There is no bounding box assigned to this chat box", "Please exit play mode and click on the Ultimate Chat Box in your scene. This will ensure that the bounding box object is created", gameObject.name ) );

				// Return the boundingBox RectTransform.
				return visibleChatBoundingBox;
			}
		}
		[SerializeField] [Tooltip( "The horizontal bounds for the content of the chat box." )]
		private RectTransform chatContentBox;
		/// <summary>
		/// Returns the RectTransform that contains the text objects for the chat box.
		/// </summary>
		public RectTransform ChatContentBox
		{
			get
			{
				// If the contentBox is null, inform the user on how to fix the issue. This should never happen though.
				if( chatContentBox == null )
					Debug.LogError( FormatDebug( "There is no content box assigned to this chat box", "Please exit play mode and click on the Ultimate Chat Box in your scene. This will ensure that the content box object is created", gameObject.name ) );

				// Return the contentBox RectTransform.
				return chatContentBox;
			}
		}
		[SerializeField] [Tooltip( "The horizontal spacing for the left and right of the content box." )] [Range( 0.0f, 50.0f )]
		private float horizontalSpacing = 2.0f;
		[SerializeField] [Tooltip( "The vertical spacing for the top and bottom of the bounding box." )] [Range( 0.0f, 50.0f )]
		private float verticalSpacing = 2.0f;
		[SerializeField] [Tooltip( "The position of the content within the chat box." )]
		private Vector2 contentPosition = Vector2.zero;
		[SerializeField] [HideInInspector]
		private Rect chatBoxScreenRect = new Rect();
		public Rect chatBoxVisiblityRect = new Rect();
		bool chatBoxPositionCustom = false;
		Vector2 bottomContentPosition;

		// TEXT SETTINGS //
		[SerializeField] [Tooltip( "The TextMeshPro GameObject to use as base settings for all the chats in the chat box." )]
		private TextMeshProUGUI textObject;
		/// <summary>
		/// Returns the TextMeshPro GameObject used as the basis for all the chats in the chat box.
		/// </summary>
		public TextMeshProUGUI TextObject
		{
			get
			{
				return textObject;
			}
		}
		[SerializeField] [Tooltip( "The color of the text in the chat box." )]
		private Color textColor = Color.white;
		/// <summary>
		/// The color of the text in the chat box. Assigning a value here will update all the text in the chat box.
		/// </summary>
		public Color TextColor
		{
			get
			{
				// Return the current textColor.
				return textColor;
			}
			set
			{
				// Assign the provided value to the stored textColor.
				textColor = value;

				// Loop through all the text objects and apply the color.
				for( int i = 0; i < AllTextObjects.Count; i++ )
					AllTextObjects[ i ].color = textColor;
			}
		}
		[SerializeField] [Tooltip( "The maximum number of chats in the chat box before the old ones will be filtered out." )]
		private int maxTextInChatBox = 500;
		[SerializeField] [Tooltip( "Determines if the players should be able to use custom rich text in the input field or if the chat box should filter out the rich text." )]
		private bool disableRichTextFromPlayers = true;
		[SerializeField] [Tooltip( "The string to add at the end of a username." )]
		private string usernameFollowup = ": ";
		[SerializeField] [Tooltip( "The space between each chat registered to the chat box." )] [Range( 0.0f, 1.0f )]
		private float spaceBetweenChats = 0.0f;
		[SerializeField] [Range( 0.0f, 0.2f )] [Tooltip( "The relative size of the font to the chat box height." )]
		private float smartFontSize = 0.1f;
		/// <summary>
		/// Returns the calculated font size based off the user defined Smart Font Size percentage.
		/// </summary>
		public float CalculatedFontSize
		{
			get
			{
				// If the text object is unassigned, then just return zero.
				if( textObject == null )
					return 0.0f;

				// Return the current font size of the base text object.
				return textObject.fontSize;
			}
		}

		// INTERACTABLE USERNAME //
		[SerializeField] [Tooltip( "Should the usernames of the chats registered by interactable?" )]
		private bool useInteractableUsername = false;
		[SerializeField] [Tooltip( "The image used in association with highlighting the username." )]
		private Image interactableUsernameImage;
		/// <summary>
		/// The image component of the username highlight.
		/// </summary>
		public Image InteractableUsernameImage
		{
			get
			{
				return interactableUsernameImage;
			}
		}
		[SerializeField] [Tooltip( "The color of the highlight image when the input is hovering over the username." )]
		private Color interactableUsernameColor = Color.white;
		/// <summary>
		/// The color of the interactable username image. Assigning a value here will update image if there is a username currently being hovered over.
		/// </summary>
		public Color InteractableUsernameColor
		{
			get
			{
				// Return the current usernameHighlightColor.
				return interactableUsernameColor;
			}
			set
			{
				// Assign the provided value to the stored usernameHighlightColor.
				interactableUsernameColor = value;

				// If a username is currently hovered, and the image is assigned, then apply the new color.
				if( UsernameHighlighted && interactableUsernameImage != null )
					interactableUsernameImage.color = interactableUsernameColor;
			}
		}
		[SerializeField] [Tooltip( "The percentage of the line height to add as a modifier to the username highlight image." )] [Range( 0.0f, 1.0f )]
		private float interactableUsernameWidthModifier = 0.0f;
		/// <summary>
		/// The current state of the player hovering over a username in the chat box.
		/// </summary>
		public bool UsernameHighlighted { get; private set; }
		/// <summary>
		/// The index of the ChatInformation that is currently being hovered.
		/// </summary>
		public int CurrentHoveredChatIndex { get; private set; }

		// FADE WHEN DISABLED //
		[SerializeField] [Tooltip( "Should the chat box fade the alpha of the CanvasGroup when enabling/disabling the chat box?" )]
		private bool fadeWhenDisabled = false;
		[SerializeField] [Tooltip( "The speed for the chat box to fade in." )]
		private float fadeInSpeed = 4.0f;
		[SerializeField] [Tooltip( "The speed for the chat box to fade out." )]
		private float fadeOutSpeed = 4.0f;
		[SerializeField] [Range( 0.0f, 1.0f )] [Tooltip( "The alpha to apply to the chat box when it is disabled." )]
		private float toggledAlpha = 0.25f;
		/// <summary>
		/// Determines if the text inside the chat box should remain fully visible even when the chat box itself is disabled.
		/// </summary>
		public bool LeaveTextVisible
		{
			get
			{
				return visibleChatBoundingBox.GetComponent<CanvasGroup>().ignoreParentGroups;
			}
			set
			{
				visibleChatBoundingBox.GetComponent<CanvasGroup>().ignoreParentGroups = value;
			}
		}
		bool fadeIn = false, fadeOut = false;
		float fadeLerpValue = 0.0f;

		// COLLAPSE WHEN DISABLED //
		[SerializeField] [Tooltip( "Should the chat box collapse when enabling/disabling the chat box?" )]
		private bool collapseWhenDisabled = false;
		[SerializeField] [Tooltip( "The speed for the chat box to expand." )]
		private float expandSpeed = 4.0f;
		[SerializeField] [Tooltip( "The speed for the chat box to collapse." )]
		private float collapseSpeed = 4.0f;
		[SerializeField] [Tooltip( "How many lines of chat should be visible when the chat box is in a collapsed state?" )]
		private int visibleLineCount = 3;
		Vector2 baseTransformSize = Vector2.zero;
		Vector2 baseTransformCollapsedSize = Vector2.zero;
		Vector2 visibleBoundingBoxSize = Vector2.zero;
		Vector2 boundingBoxCollapsedSize = Vector2.zero;
		bool expandChatBox = false, collapseChatBox = false;
		float collapseLerpValue;

		// TEXT EMOJI //
		[SerializeField] [Tooltip( "Should emojis be allowed in chat?" )]
		private bool useTextEmoji = false;
		[SerializeField] [Tooltip( "The emoji asset to assign to all the chats." )]
		private TMP_SpriteAsset emojiAsset;
		/// <summary>
		/// Returns the assigned TextMeshPro sprite asset.
		/// </summary>
		public TMP_SpriteAsset EmojiAsset
		{
			get
			{
				return emojiAsset;
			}
		}

		// VERTICAL SCROLLBAR //
		[SerializeField] [Tooltip( "Should the chat box display a vertical scrollbar to help navigate chat?" )]
		private bool useScrollbar = false;
		[SerializeField]
		private RectTransform scrollbarBase = null, scrollbarHandle = null;
		[SerializeField]
		private Image scrollbarHandleImage;
		[SerializeField] [Tooltip( "The default color for the scrollbar handle." )]
		private Color scrollbarHandleNormalColor = Color.white;
		[SerializeField] [Tooltip( "The color for when the input is hovering the scrollbar handle." )]
		private Color scrollbarHandleHoverColor = Color.white;
		[SerializeField] [Tooltip( "The color of the scrollbar handle when the input is active." )]
		private Color scrollbarHandleActiveColor = Color.white;
		[SerializeField] [Tooltip( "Should the player be able to use the scroll wheel on their mouse to navigate the chat box?" )]
		private bool useScrollWheel = true;
		[SerializeField] [Tooltip( "The speed that the chat box will navigate using the scroll wheel." )]
		private float mouseScrollSpeed = 1.0f;
		[SerializeField] [Tooltip( "The minimum size that the scrollbar handle will get when the chat box is filled with chats." )] [Range( 0.01f, 0.25f )]
		private float scrollbarMinimumSize = 0.15f;
		[SerializeField] [Tooltip( "The width of the scrollbar relative to the chat box width." )] [Range( 0.01f, 0.25f )]
		private float scrollbarWidth = 0.02f;
		[SerializeField] [Tooltip( "The horizontal position of the scrollbar relative to the chat box center." )]
		private float scrollbarHorizontalPosition = 49.0f;
		[SerializeField] [Tooltip( "Should the scrollbar only be visible when hovering input over the chat box?" )]
		private bool visibleOnlyOnHover = false;
		[SerializeField] [Tooltip( "The speed to apply for the scrollbar to toggle visually." )]
		private float scrollbarToggleSpeed = 4.0f;
		[SerializeField] [Tooltip( "The time in seconds that the input has NOT been in the chat box for the scrollbar to disable itself." )]
		private float scrollbarInactiveTime = 1.0f;
		[SerializeField] [Tooltip( "Should clicking on the base of the scrollbar to navigate be disabled?" )]
		private bool disableBaseNavigation = false;
		[SerializeField] [HideInInspector]
		private Rect scrollbarBaseRect = new Rect(), scrollbarHandleRect = new Rect();
		[SerializeField] [HideInInspector]
		private CanvasGroup scrollbarCanvasGroup;
		/// <summary>
		/// The current state of the scrollbar being visible or not.
		/// </summary>
		public bool ScrollbarActive { get; private set; }
		Vector2 scrollbarHandleInputStart = Vector2.zero;
		Vector2 scrollbarBottomPosition;
		float scrollbarToggleLerpValue = 0.0f, _inactiveTime = 0.0f;
		bool scrollbarToggle = false, scrollbarHandleHovered = false, scrollbarHandleActive = false;

		// NAVIGATION ARROWS //
		[SerializeField] [Tooltip( "Determines if you want to have navigation arrows at the top and bottom of the scrollbar to help navigate the chat box." )]
		private bool useNavigationArrows = false;
		[SerializeField] [Tooltip( "The image component to use for the up arrow." )]
		private Image navigationArrowUp;
		[SerializeField] [Tooltip( "The image component to use for the down arrow." )]
		private Image navigationArrowDown;
		[SerializeField] [Tooltip( "The default color for the navigation arrows to be." )]
		private Color navigationNormalColor = Color.white;
		[SerializeField] [Tooltip( "The color to apply when the navigation arrows are hovered." )]
		private Color navigationHoverColor = Color.white;
		[SerializeField] [Tooltip( "The color to apply when the input is active on the navigation arrows." )]
		private Color navigationActiveColor = Color.white;
		[SerializeField] [Tooltip( "The time in seconds before the navigation arrows will repeat the navigation when the input is held." )]
		private float navigationInitialHoldDelay = 0.25f;
		[SerializeField] [Tooltip( "The time in seconds between repeating navigation from holding the input on the navigation arrows." )]
		private float navigationIntervalDelay = 0.1f;
		[SerializeField] [HideInInspector]
		private Rect scrollbarNavigationArrowUpRect = new Rect(), scrollbarNavigationArrowDownRect = new Rect();
		bool scrollbarNavigationArrowUpHovered = false, scrollbarNavigationArrowUpActivated = false;
		bool scrollbarNavigationArrowDownHovered = false, scrollbarNavigationArrowDownActivated = false;
		float navigationIntervalTime = 0.0f;
		float navigationInitialHoldTime = 0.0f;

		// INPUT FIELD //
		[SerializeField] [Tooltip( "Should an input field be available for players to use?" )]
		private bool useInputField = false;
		[SerializeField] [Tooltip( "The input field component to use for the chat box." )]
		private TMP_InputField inputField = null;
		/// <summary>
		/// The input field component used in connection with the chat box.
		/// </summary>
		public TMP_InputField InputField
		{
			get
			{
				return inputField;
			}
		}
		[SerializeField] [Tooltip( "The size of the input field relative to the chat box." )]
		private Vector2 inputFieldSize = new Vector2( 100, 12.5f );
		[SerializeField] [Tooltip( "The position of the input field relative to the chat box." )]
		private Vector2 inputFieldPosition = new Vector2( 0, -2 );
		[SerializeField] [Range( 0.0f, 1.0f )] [Tooltip( "The relative size of the font to the input field height." )]
		private float inputFieldSmartFontSize = 1.0f;
		[SerializeField] [Tooltip( "The size of the input field text area relative to the input field transform." )]
		private Vector2 inputFieldTextAreaSize = new Vector2( 95.0f, 95.0f );
		[SerializeField] [Range( -50.0f, 50.0f )]
		private float inputFieldTextHorizontalPosition = 0.0f;
		[SerializeField] [HideInInspector]
		private RectTransform inputFieldTransform;
		[SerializeField] [HideInInspector]
		private Rect inputFieldRect = new Rect();
		/// <summary>
		/// Returns the current state of the input field having focus or not.
		/// </summary>
		public bool InputFieldEnabled { get; private set; }
		string inputFieldValue = string.Empty;
		/// <summary>
		/// The current string value of the input field.
		/// </summary>
		public string InputFieldValue
		{
			get => inputFieldValue;
			set
			{
				inputFieldValue = value;
				inputField.text = inputFieldValue;
				inputField.caretPosition = inputFieldValue.Length;
			}
		}
		/// <summary>
		/// Returns if the current input field value contains a command value or not.
		/// </summary>
		public bool InputFieldContainsCommand { get; private set; }

		// EXTRA IMAGE //
		[SerializeField] [Tooltip( "Should an extra image be used inside the input field?" )]
		private bool useExtraImage = false;
		[SerializeField] [Tooltip( "The image component to use as the extra image." )]
		private Image extraImage;
		[SerializeField] [HideInInspector]
		private Rect extraImageRect = new Rect();
		/// <summary>
		/// The sprite associated with the extra image.
		/// </summary>
		public Sprite ExtraImageSprite
		{
			get
			{
				if( !useExtraImage || extraImage == null )
					return null;

				return extraImage.sprite;
			}
			set
			{
				if( useExtraImage && extraImage != null )
					extraImage.sprite = value;
			}
		}
		/// <summary>
		/// The extra image color.
		/// </summary>
		public Color ExtraImageColor
		{
			get
			{
				if( !useExtraImage || extraImage == null )
					return Color.clear;

				return extraImage.color;
			}
			set
			{
				if( useExtraImage && extraImage != null )
					extraImage.color = value;
			}
		}
		[SerializeField] [Range( 0.0f, 100.0f )] [Tooltip( "The height of the input field extra image." )]
		private float extraImageHeight = 100.0f;
		[SerializeField] [Range( 0.0f, 100.0f )] [Tooltip( "The width of the input field extra image." )]
		private float extraImageWidth = 10.0f;
		[SerializeField] [Range( -50.0f, 50.0f )] [Tooltip( "The horizontal position in relation to the input field." )]
		private float extraImageHorizontalPosition = -50.0f;

		// EMOJI WINDOW //
		[SerializeField] [Tooltip( "Should the players be able to add emojis through a provided window?" )]
		private bool useEmojiWindow = false;
		[SerializeField] [Tooltip( "The image component to be used as a button to open the emoji window." )]
		private Image emojiButtonImage;
		[SerializeField] [Tooltip( "The size of the button." )]
		private float emojiButtonSize = 0.9f;
		[SerializeField] [Range( 0.0f, 150.0f )] [Tooltip( "The horizontal position of the emoji button in relation to the left side of the input field." )]
		private float emojiButtonHorizontalPosition = 99.0f;
		[SerializeField] [Tooltip( "The image component used as the background of the emoji window." )]
		private Image emojiWindowImage;
		[SerializeField] [Tooltip( "The overall size of the emoji window." )]
		private Vector2 emojiWindowSize = new Vector2( 25, 25 );
		[SerializeField] [Tooltip( "The position of the emoji window in relation to the bottom right of the input field." )]
		private Vector2 emojiWindowPosition = Vector2.zero;
		[SerializeField] [Tooltip( "The text to display all the available emojis." )]
		private TextMeshProUGUI emojiText;
		[SerializeField] [Range( 0.0f, 0.2f )] [Tooltip( "The padding to add to the edges of the emoji window text." )]
		private float emojiTextEdgePadding = 0.0f;
		[SerializeField] [Tooltip( "How many emojis should be displayed in a row inside the window?" )]
		private int emojiPerRow = 5;
		[SerializeField] [HideInInspector]
		private List<Rect> emojiRects = new List<Rect>();
		[SerializeField] [HideInInspector]
		private Rect emojiButtonRect = new Rect(), emojiWindowRect = new Rect();
		[SerializeField] [HideInInspector]
		private CanvasGroup emojiWindowCanvasGroup;
		/// <summary>
		/// Returns the current state of the emoji window being enabled and interactable.
		/// </summary>
		public bool EmojiWindowEnabled { get; private set; } = true;
		/// <summary>
		/// The sprite of the emoji button on the chat box. 
		/// </summary>
		public Sprite EmojiButtonSprite
		{
			get
			{
				if( !useEmojiWindow || emojiButtonImage == null )
					return null;

				return emojiButtonImage.sprite;
			}
			set
			{
				if( useEmojiWindow && emojiButtonImage != null )
					emojiButtonImage.sprite = value;
			}
		}
		/// <summary>
		/// The color of the emoji button on the chat box.
		/// </summary>
		public Color EmojiButtonColor
		{
			get
			{
				// If the user doesn't want to use an emoji window, or the emoji button is unassigned, return a clear color.
				if( !useEmojiWindow || emojiButtonImage == null )
					return Color.clear;

				// Otherwise, return the emoji button's color.
				return emojiButtonImage.color;
			}
			set
			{
				// If the user does want to use a window for the emojis and the button is assigned, assign the emoji button color to the provided value.
				if( useEmojiWindow && emojiButtonImage != null )
					emojiButtonImage.color = value;
			}
		}

		/*[Serializable] public class ChatStyle
		{
			public bool usernameBold = false;
			public bool usernameItalic = false;
			public bool usernameUnderlined = false;
			public Color usernameColor = Color.clear;
			public bool disableInteraction = false;
			public bool noUsernameFollowupText = false;
			public bool messageBold = false;
			public bool messageItalic = false;
			public bool messageUnderlined = false;
			public Color messageColor = Color.clear;

			/// <summary>
			/// [INTERNAL] Formats the content of the chat according to the style settings.
			/// </summary>
			public string FormatMessage ( string username, string message )
			{
				if( username != string.Empty )
				{
					if( usernameBold )
						username = "<b>" + username + "</b>";

					if( usernameItalic )
						username = "<i>" + username + "</i>";

					if( usernameUnderlined )
						username = "<u>" + username + "</u>";

					if( usernameColor != Color.clear )
						username = $"<color=#{ColorUtility.ToHtmlStringRGB( usernameColor )}>" + username + "</color>";
				}

				if( messageBold )
					message = "<b>" + message + "</b>";

				if( messageItalic )
					message = "<i>" + message + "</i>";

				if( messageUnderlined )
					message = "<u>" + message + "</u>";

				if( messageColor != Color.clear )
					message = $"<color=#{ColorUtility.ToHtmlStringRGB( messageColor )}>" + message + "</color>";

				return username + message;
			}

			/// <summary>
			/// [INTERNAL] Formats the username only for calculations.
			/// </summary>
			public string FormatUsernameOnly ( string username )
			{
				if( username.Contains( "#" ) )
					username = username.Split( '#' )[ 0 ];

				if( usernameBold )
					username = "<b>" + username + "</b>";

				if( usernameItalic )
					username = "<i>" + username + "</i>";

				if( usernameUnderlined )
					username = "<u>" + username + "</u>";

				return username;
			}
		}#1#


    #endregion


    #region Callbacks
// CALLBACKS //
    /// <summary>
    /// This event is called when a chat has been registered to the chat box.
    /// </summary>
    public event Action<ChatInformation> OnChatRegistered;
    /// <summary>
    /// Callback for when a new username has been hovered by the player.
    /// </summary>
    public event Action<Vector2, ChatInformation> OnUsernameHover;
    /// <summary>
    /// Callback for when the player has interacted with a username.
    /// </summary>
    public event Action<Vector2, ChatInformation> OnUsernameInteract;
    /// <summary>
    /// Callback for when the input field has been enabled.
    /// </summary>
    public event Action OnInputFieldEnabled;
    /// <summary>
    /// Callback for when the input field has been disabled.
    /// </summary>
    public event Action OnInputFieldDisabled;
    /// <summary>
    /// This event is called when the input field of the chat box has been updated.
    /// </summary>
    public event Action<string> OnInputFieldUpdated;
    /// <summary>
    /// This event is called when the input field of the chat box has been submitted.
    /// </summary>
    public event Action<string> OnInputFieldSubmitted;
    /// <summary>
    /// This event is called when the input field is updated and contains a potential command.
    /// </summary>
    public event Action<string, string> OnInputFieldCommandUpdated;
    /// <summary>
    /// This event is called when the input field is submitted and contains a potential command.
    /// </summary>
    public event Action<string, string> OnInputFieldCommandSubmitted;
    /// <summary>
    /// This event is called when the extra image associated with the input field has been interacted with.
    /// </summary>
    public event Action OnExtraImageInteract;
    

    #endregion

    
    #region TextMethods

    /// <summary>
    /// [INTERNAL] Formats and sends detailed information to the user.
    /// </summary>
    private static string FormatDebug ( string error, string solution, string objectName )
    {
	    return "<b>Ultimate Chat Box</b>\n" +
	           "<color=red><b>×</b></color> <i><b>Error:</b></i> " + error + ".\n" +
	           "<color=green><b>√</b></color> <i><b>Solution:</b></i> " + solution + ".\n" +
	           "<color=cyan><b>∙</b></color> <i><b>Object:</b></i> " + objectName + "\n";
    }
    
    public void SendTextToPool ( TextMeshProUGUI chatText )
    {
	    // Add the text to the pool.
	    UnusedTextPool.Add( chatText );

	    // Make the text invisible.
	    chatText.color = Color.clear;
    }

    #endregion


    #region Function

    

    #endregion
    */


    
}

/*
[Serializable]
public class ChatInformation
{
	public UdonUltimateChatBox chatBox;
         
	/// <summary>
	/// Returns the current state of this chat being visible.
	/// </summary>
	public bool IsVisible
	{
		get;
		private set;
	}
	public TextMeshProUGUI chatText;
	/// <summary>
	/// The exact username that was provided when registering a chat. If the provided username contained a number, this string includes that number for reference.
	/// </summary>
	public string Username { get; set; }
	/// <summary>
	/// The display username. If the username provided when registering a chat contained a number, then this string will be just the name portion, excluding the associated number.
	/// </summary>
	public string DisplayUsername { get; set; }
	/// <summary>
	/// The exact string value provided to the chat box for the message.
	/// </summary>
	public string Message { get; set; }
	/// <summary>
	/// The modified string value with all the users options applied to it so that it looks correct.
	/// </summary>
	public string DisplayMessage { get; set; }
	public Rect usernameRect = new Rect();
	public float contentSpace = 0.0f;
	public Vector2 anchoredPosition = Vector2.zero;
	public float usernameWidth = 0.0f;
	public float lineHeight = 0.0f;
	public int lineCount = 0;
	public ChatStyle chatBoxStyle;
         
         
	/// <summary>
	/// [INTERNAL] Updates the text component of this chat so that it can be display.
	/// </summary>
	public void UpdateText ()
	{
		// If the assigned text is null, or the provided style is null, return.
		if( chatText == null || chatBoxStyle == null )
			return;
         
		// Update the text component to display the chat.
		chatText.text = chatBoxStyle.FormatMessage( DisplayUsername, DisplayMessage );
	}
         
	/// <summary>
	/// [INTERNAL] Updates the visibility of this chat.
	/// </summary>
	public void UpdateVisibility ()
	{
		// Check the top position of the text and if the chat box contains the position then set IsVisible to true.
		if( chatBox.chatBoxVisiblityRect.Contains( anchoredPosition * -1 ) )
			IsVisible = true;
		// Else check the position of the text + the content space, and if it is within the chat box then set IsVisible to true.
		else if( chatBox.chatBoxVisiblityRect.Contains( ( anchoredPosition * -1 ) + new Vector2( 0, contentSpace - 0.01f ) ) )
			IsVisible = true;
		// Else if the content space is actually larger than the visible bounds of the chat box... 
		else if( contentSpace > chatBox.visibleChatBoundingBox.sizeDelta.y )
		{
			// Calculate difference of the visible bounds of the chat box and the space of this content.
			float differenceModifier = chatBox.visibleChatBoundingBox.sizeDelta.y / contentSpace;
         
			// Loop for adjusting the checks for the content space to see if this chat is indeed visible...
			for( float mod = 0.0f; mod < 1.0f; mod += differenceModifier )
			{
				// If the chat box contains the content spaced * mod above, then set IsVisible to true and return.
				if( chatBox.chatBoxVisiblityRect.Contains( ( anchoredPosition * -1 ) + new Vector2( 0, contentSpace * mod ) ) )
				{
					IsVisible = true;
					return;
				}
			}
         
			// Else set IsVisible to false.
			IsVisible = false;
		}
		// Else none of the other checks were true so the chat isn't visible. Set IsVisible to false.
		else
			IsVisible = false;
	}
         
	/// <summary>
	/// !!!Removes this chat information from the chat box.
	/// </summary>
	public void RemoveChat ()
	{
		// If the chat text is assigned, then send it to the pool to be reused.
		if( chatText != null )
			chatBox.SendTextToPool( chatText );
         
		// Remove this chat information from the list.
		//chatBox.ChatInformations.Remove( this );
         
		// Set isDirty to true so the chat box knows to update the positioning of everything.
		chatBox.isDirty = true;
	}
}

[Serializable]
public class ChatStyle
{
	public bool usernameBold = false;
	public bool usernameItalic = false;
	public bool usernameUnderlined = false;
	public Color usernameColor = Color.clear;
	public bool disableInteraction = false;
	public bool noUsernameFollowupText = false;
	public bool messageBold = false;
	public bool messageItalic = false;
	public bool messageUnderlined = false;
	public Color messageColor = Color.clear;

	/// <summary>
	/// [INTERNAL] Formats the content of the chat according to the style settings.
	/// </summary>
	public string FormatMessage ( string username, string message )
	{
		if( username != string.Empty )
		{
			if( usernameBold )
				username = "<b>" + username + "</b>";

			if( usernameItalic )
				username = "<i>" + username + "</i>";

			if( usernameUnderlined )
				username = "<u>" + username + "</u>";

			if( usernameColor != Color.clear )
				username = $"<color=#{ColorUtility.ToHtmlStringRGB( usernameColor )}>" + username + "</color>";
		}

		if( messageBold )
			message = "<b>" + message + "</b>";

		if( messageItalic )
			message = "<i>" + message + "</i>";

		if( messageUnderlined )
			message = "<u>" + message + "</u>";

		if( messageColor != Color.clear )
			message = $"<color=#{ColorUtility.ToHtmlStringRGB( messageColor )}>" + message + "</color>";

		return username + message;
	}

	/// <summary>
	/// [INTERNAL] Formats the username only for calculations.
	/// </summary>
	public string FormatUsernameOnly ( string username )
	{
		if( username.Contains( "#" ) )
			username = username.Split( '#' )[ 0 ];

		if( usernameBold )
			username = "<b>" + username + "</b>";

		if( usernameItalic )
			username = "<i>" + username + "</i>";

		if( usernameUnderlined )
			username = "<u>" + username + "</u>";

		return username;
	}
}
*/
