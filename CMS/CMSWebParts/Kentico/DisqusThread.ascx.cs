using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

using CMS.Base;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalControls;

public partial class CMSWebParts_Kentico_DisqusThread : CMSAbstractWebPart
{
	#region "Backing fields"

	private static readonly Lazy<string> mDisqusSecretAPIKey = new Lazy<string>(() => SettingsHelper.AppSettings["DisqusSecretAPIKey"]);
	private static readonly Lazy<string> mDisqusShortName = new Lazy<string>(() => SettingsHelper.AppSettings["DisqusShortName"]);

	#endregion


	#region "Properties (according to https://help.disqus.com/customer/portal/articles/472098-javascript-configuration-variables)"

	public static string DisqusSecretAPIKey
	{
		get { return mDisqusSecretAPIKey.Value; }
	}


	/// <summary>
	/// Gets or sets the Disqus Shortname. Tells the Disqus service your forum's shortname.
	/// </summary>
	public string DisqusShortName
	{
		get
		{
			return DataHelper.GetNotEmpty(mDisqusShortName.Value, ValidationHelper.GetString(GetValue("DisqusShortName"), string.Empty));
		}
		set
		{
			SetValue("DisqusShortName", value);
		}
	}


	/// <summary>
	/// Gets or sets the Disqus Identifier. Tells the Disqus service how to identify the current page. Can be a string or an integer.
	/// </summary>
	public string DisqusIdentifier
	{
		get
		{
			return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("DisqusIdentifier"), string.Empty), DocumentContext.CurrentAliasPath);
		}
		set
		{
			SetValue("DisqusIdentifier", value);
		}
	}


	/// <summary>
	/// Gets or sets the Disqus URL. Tells the Disqus service the URL of the current page. If undefined, Disqus will take the window.location.href.
	/// </summary>
	public string DisqusUrl
	{
		get
		{
			return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("DisqusUrl"), string.Empty), URLHelper.GetAbsoluteUrl(DocumentContext.CurrentAliasPath));
		}
		set
		{
			SetValue("DisqusUrl", value);
		}
	}


	/// <summary>
	/// Gets or sets the Disqus Title. Tells the Disqus service the title of the current page. This is used when creating the thread on Disqus for the first time. If undefined, Disqus will use the title attribute of the page.
	/// </summary>
	public string DisqusTitle
	{
		get
		{
			return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("DisqusTitle"), string.Empty), DocumentContext.CurrentDocument.DocumentName);
		}
		set
		{
			SetValue("DisqusTitle", value);
		}
	}


	/// <summary>
	/// Gets or sets the Disqus Identifier. Tells the Disqus service how to identify the current page. Can be a string or an integer.
	/// </summary>
	public int DisqusCategoryId
	{
		get
		{
			return ValidationHelper.GetInteger(GetValue("DisqusCategoryId"), 0);
		}
		set
		{
			SetValue("DisqusCategoryId", value);
		}
	}


	/// <summary>
	/// Gets or sets additional Disqus configuration options (disqus_config).
	/// </summary>
	public string DisqusConfig
	{
		get
		{
			return ValidationHelper.GetString(GetValue("DisqusConfig"), null);
		}
		set
		{
			SetValue("DisqusConfig", value);
		}
	}


	#region "Single sign-on (according to https://help.disqus.com/customer/portal/articles/236206-integrating-single-sign-on)"

	/// <summary>
	/// Gets or sets whether to enable single sign-on functionality.
	/// </summary>
	public bool EnableSingleSignOn
	{
		get
		{
			return ValidationHelper.GetBoolean(GetValue("EnableSingleSignOn"), false);
		}
		set
		{
			SetValue("EnableSingleSignOn", value);
		}
	}


	/// <summary>
	/// Gets or sets the Disqus Public API Key.
	/// </summary>
	public string DisqusPublicAPIKey
	{
		get
		{
			return ValidationHelper.GetString(GetValue("DisqusPublicAPIKey"), null);
		}
		set
		{
			SetValue("DisqusPublicAPIKey", value);
		}
	}

	#region "Button"

	/// <summary>
	/// Gets or sets the site name. (Displayed it in the Post As window.)
	/// </summary>
	public string DisqusButtonSiteName
	{
		get
		{
			return ValidationHelper.GetString(GetValue("DisqusButtonSiteName"), null);
		}
		set
		{
			SetValue("DisqusButtonSiteName", value);
		}
	}


	/// <summary>
	/// Gets or sets the button image URL.
	/// </summary>
	public string DisqusButtonImageUrl
	{
		get
		{
			return ValidationHelper.GetString(GetValue("DisqusButtonImageUrl"), null);
		}
		set
		{
			SetValue("DisqusButtonImageUrl", value);
		}
	}


	/// <summary>
	/// Gets or sets address of your login page. The page will be opened in a new window and it must close itself after authentication is done. That's how we know when it is done and reload the page.
	/// </summary>
	public string DisqusLoginUrl
	{
		get
		{
			return ValidationHelper.GetString(GetValue("DisqusLoginUrl"), null);
		}
		set
		{
			SetValue("DisqusLoginUrl", value);
		}
	}


	/// <summary>
	/// Gets or sets address of your logout page. This page must redirect user back to the original page after logout.
	/// </summary>
	public string DisqusLogoutUrl
	{
		get
		{
			return ValidationHelper.GetString(GetValue("DisqusLogoutUrl"), null);
		}
		set
		{
			SetValue("DisqusLogoutUrl", value);
		}
	}


	/// <summary>
	/// Gets or sets width of the login popup window. Default is 800.
	/// </summary>
	public int DisqusLoginWindowWidth
	{
		get
		{
			return ValidationHelper.GetInteger(GetValue("DisqusLoginWindowWidth"), 800);
		}
		set
		{
			SetValue("DisqusLoginWindowWidth", value);
		}
	}


	/// <summary>
	/// Gets or sets height of the login popup window. Default is 400.
	/// </summary>
	public int DisqusLoginWindowHeight
	{
		get
		{
			return ValidationHelper.GetInteger(GetValue("DisqusLoginWindowHeight"), 400);
		}
		set
		{
			SetValue("DisqusLoginWindowHeight", value);
		}
	}

	#endregion


	#endregion


	#endregion


	/// <summary>
	/// Content loaded event handler
	/// </summary>
	public override void OnContentLoaded()
	{
		base.OnContentLoaded();
		SetupControl();
	}


	/// <summary>
	/// Initializes the control properties
	/// </summary>
	protected void SetupControl()
	{
		if (StopProcessing)
		{
			// Do nothing
			plcContent.Visible = false;
		}
		else
		{
			ReloadData();
		}
	}


	/// <summary>
	/// Reload data
	/// </summary>
	public override void ReloadData()
	{
		StringBuilder config = new StringBuilder();
		if (!string.IsNullOrEmpty(DisqusUrl))
		{
			config.AppendLine("this.page.url = " + ScriptHelper.GetString(DisqusUrl) + ";");
		}
		if (!string.IsNullOrEmpty(DisqusIdentifier))
		{
			config.AppendLine("this.page.identifier = " + ScriptHelper.GetString(DisqusIdentifier) + ";");
		}
		if (!string.IsNullOrEmpty(DisqusTitle))
		{
			config.AppendLine("this.page.title = " + ScriptHelper.GetString(DisqusTitle) + ";");
		}
		if (DisqusCategoryId > 0)
		{
			config.AppendLine("this.page.category_id = " + DisqusCategoryId + ";");
		}
		if (EnableSingleSignOn && CurrentUser.GetValue("UserAllowDisqusSSO", true))
		{
			config.AppendLine("this.page.remote_auth_s3 = " + ScriptHelper.GetString(GetPayload()) + ";");
			config.AppendLine("this.page.api_key = " + ScriptHelper.GetString(DisqusPublicAPIKey) + ";");
			config.AppendLine("this.sso = " + GetButton() + ";");
		}
		if (!string.IsNullOrEmpty(DisqusConfig))
		{
			config.AppendLine(DisqusConfig);
		}

		string script = string.Format(@"
var disqus_config = function () {{
	{0}
}};
    
(function() {{
    var d = document, s = d.createElement('script');        
    s.src = '//{1}.disqus.com/embed.js';        
    s.setAttribute('data-timestamp', +new Date());
    (d.head || d.body).appendChild(s);
}})();
", config, DisqusShortName);

		ltlScript.Text = ScriptHelper.GetScript(script);
	}

	#region "Single sign-on"

	/// <summary>
	/// Generates an object with login button settings.
	/// </summary>
	/// <returns>JSON object</returns>
	private string GetButton()
	{
		var button = new
		{
			name = DisqusButtonSiteName,
			button = DisqusButtonImageUrl,
			url = DisqusLoginUrl,
			logout = DisqusLogoutUrl,
			width = DisqusLoginWindowWidth.ToString(),
			height = DisqusLoginWindowHeight.ToString()

		};
		return new JavaScriptSerializer().Serialize(button);
	}


	/// <summary>
	/// Gets the Disqus SSO payload based on whether the user is authenticated or not
	/// </summary>
	/// <returns>SSO payload</returns>
	private string GetPayload()
	{
		var user = CurrentUser;
		if (!user.IsPublic())
		{
			//TODO: Improve user profile URL (take from "Member profile path" setting) + parametrize user properties
			return GetPayloadForAuthenticatedUser(user.UserID.ToString(), user.FullName, user.Email, 
				URLHelper.GetAbsoluteUrl(AvatarInfoProvider.GetUserAvatarImageUrl(user.UserAvatarID, null, null, 0, 0, 0)),
				URLHelper.GetAbsoluteUrl("TODO"));
		}
		else
		{
			return GetPayloadForAnonymousUser();
		}
	}


	/// <summary>
	/// Gets the Disqus SSO payload to authenticate users
	/// </summary>
	/// <param name="userId">The unique ID to associate with the user</param>
	/// <param name="userName">Non-unique name shown next to comments.</param>
	/// <param name="userEmail">User's email address, defined by RFC 5322</param>
	/// <param name="userAvatarUrl">URL of the avatar image</param>
	/// <param name="userProfileUrl">Website, blog or custom profile URL for the user, defined by RFC 3986</param>
	/// <returns>A string containing the signed payload</returns>
	private static string GetPayloadForAuthenticatedUser(string userId, string userName, string userEmail, string userAvatarUrl = "", string userProfileUrl = "")
	{
		var userdata = new
		{
			id = userId,
			username = userName,
			email = userEmail,
			avatar = userAvatarUrl,
			url = userProfileUrl
		};

		return GeneratePayload(new JavaScriptSerializer().Serialize(userdata));
	}


	/// <summary>
	/// Method to log out a user from SSO
	/// </summary>
	/// <returns>A signed, empty payload string</returns>
	private static string GetPayloadForAnonymousUser()
	{
		return GeneratePayload(new JavaScriptSerializer().Serialize(new { }));
	}


	private static string GeneratePayload(string serializedUserData)
	{
		byte[] userDataAsBytes = Encoding.UTF8.GetBytes(serializedUserData);

		// Base64 Encode the message
		string Message = Convert.ToBase64String(userDataAsBytes);

		// Get the proper timestamp
		TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
		string Timestamp = Convert.ToInt32(ts.TotalSeconds).ToString();

		// Convert the message + timestamp to bytes
		byte[] messageAndTimestampBytes = Encoding.ASCII.GetBytes(Message + " " + Timestamp);

		// Convert Disqus API key to HMAC-SHA1 signature
		byte[] apiBytes = Encoding.ASCII.GetBytes(DisqusSecretAPIKey);
		HMACSHA1 hmac = new HMACSHA1(apiBytes);
		byte[] hashedMessage = hmac.ComputeHash(messageAndTimestampBytes);

		// Put it all together into the final payload
		return Message + " " + ByteToString(hashedMessage) + " " + Timestamp;
	}

	private static string ByteToString(byte[] buff)
	{
		// Convert to HEX
		return buff.Aggregate("", (current, t) => current + t.ToString("X2"));
	}

	#endregion
}
