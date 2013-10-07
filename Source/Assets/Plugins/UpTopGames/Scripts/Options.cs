using UnityEngine;
using System.Collections;
using CodeTitans.JSon;

public class Options : MonoBehaviour 
{	
	public UISlider soundVolumeSlider;
	
	public UIStateToggleBtn pushNotifications;
	
	// Use this for initialization
	void Start ()
	{
		GetComponent<UIInteractivePanel>().transitions.list[0].AddTransitionStartDelegate(OptionsLoad);
		soundVolumeSlider.SetValueChangedDelegate(ChangeVolume);
		
	}
	
	void OptionsLoad(EZTransition transition)
	{
		soundVolumeSlider.Value = Save.GetFloat(PlayerPrefsKeys.VOLUME.ToString());
		if(Save.GetString(PlayerPrefsKeys.PUSHNOTIFICATIONS.ToString()) == "On") pushNotifications.SetState(0);
		else pushNotifications.SetState(1);
	}
	
	void ChangeVolume(IUIObject volumeSlider)
	{
		Save.Set(PlayerPrefsKeys.VOLUME.ToString(), soundVolumeSlider.Value, true);
	}
	
	void ChangePushNotificationStatus()
	{
		if(Save.HasKey(PlayerPrefsKeys.TOKEN.ToString()))
		{
			if(Save.GetString (PlayerPrefsKeys.PUSHNOTIFICATIONS.ToString()) == "Off")
			{
				WWWForm form = new WWWForm();
				form.AddField("action","enable");
				new GameJsonAuthConnection(Flow.URL_BASE+"login/push.php",HandlePushNotifications).connect(form);
			}
			else
			{
				WWWForm form = new WWWForm();
				form.AddField("action","disable");
				new GameJsonAuthConnection(Flow.URL_BASE+"login/push.php",HandlePushNotifications).connect(form);
			}
		}
		else
		{
			Flow.game_native.showMessage("Login", "You must login to perform this change");
		}
	}
	
	void HandlePushNotifications(string error, IJSonObject data)
	{
		if(error != null)
		{
			Flow.game_native.showMessage("Error", error);
		}
		else
		{
			Flow.game_native.showMessage("Push Notifications",data.StringValue);
			
			if(data.StringValue == "Push notifications enabled!") 
			{
				pushNotifications.SetState(0);
				Save.Set(PlayerPrefsKeys.PUSHNOTIFICATIONS.ToString(),"On");
			}
			else if(data.StringValue == "Push notifications disabled!") 
			{
				pushNotifications.SetState(1);
				Save.Set(PlayerPrefsKeys.PUSHNOTIFICATIONS.ToString(), "Off");
			}
		}
	}
	
	void AccountSettings()
	{
		if(Save.HasKey(PlayerPrefsKeys.TOKEN.ToString()))
		{
			UIPanelManager.instance.BringIn("AccountSettingsScenePanel", UIPanelManager.MENU_DIRECTION.Forwards);
		}
		else
		{
			UIPanelManager.instance.BringIn("LoginScenePanel",UIPanelManager.MENU_DIRECTION.Forwards);
		}
	}
	
	
}/*

FREE GAMES AND TOPP APPS TERMS OF USE
The terms of this agreement (\"Terms of Use\" or \"Terms\") govern the relationship between you and
Free Games and Top Apps (hereinafter \"Us\" or \"We\") regarding your use of ours social and
multiplayer games and related services (hereinafter \"Service\"), which include applications for mobile
devices and applications for web. In this agreement, \"Free Games and Top Apps\" means Free Games
and Top Apps LTDA. located at Rua do Gasometro n. 200, Sao Paulo, Brazil.

By creating an account in our login system or logging in with a Facebook account you accept and
agree to be bound by these Terms of Use and consent to the collection, use and storage of your
information."
We reserve the right, at our discretion, to change, modify, add or remove portions of these Terms of
Use at any time by posting the amended Terms on. You may also be given additional notice, such as
an e-mail message or messaging within the Service, of any changes. You will be deemed to have
accepted such changes by continuing to use the Service. We may also revise other policies, codes or
rules at any time.

This agreement may not be otherwise amended except in a writing hand signed by you and us. For
purposes of this provision, \"writing\" does not include an e-mail message and a signature does not
include an electronic signature.

If at any point you do not agree to any portion of the then-current version of our Terms of Use, rules
or codes of conduct relating to your use of the Service, your license to use the Service shall
immediately terminate and you must immediately stop using the Service."
"
To the extent the Terms of Use conflict with any other terms, policy, rules or codes of conduct, the
terms contained here shall govern.

Free Games and Top Apps grants you a non-exclusive, non-transferable, revocable limited license to
access and use the Service using a supported web browser (such as Mozilla Firefox or Microsoft
Internet Explorer) or mobile device with compatible operating system solely for your own non-"
commercial entertainment purposes. You agree not to use the Service for any other purpose.

You understand that while at times you may \"earn\" \"buy\" or \"purchase\" (a) virtual currency,
including but not limited to virtual coins, cash, tokens, or points, all for use in the Service; or (b)
virtual in-game items (together with virtual currency, \"Virtual Items\"); these real world terms are
only being used as shorthand. You do not in fact \"own\" the Virtual Items and the amounts of any
Virtual Item do not refer to any credit balance of real currency or its equivalent. Rather, you may
purchase a limited license to use the Service, including software programs that occasionally manifest
themselves as these items. The purchase and sale of the limited license referred to in these Terms of
Use is a completed transaction upon receipt of your direct payment or redemption of a third party
virtual currency like Facebook Credits. Any \"virtual currency\" balance shown in your Account does
not constitute a real-world balance or reflect any stored value, but instead constitutes a
measurement of the extent of your license.

By accessing or using the Service, including browsing or accessing a game, you accept and agree to
these Terms of Use. 

You warrant that you are not prohibited from receiving products of U.S. origin, including services or
software. If you are between the ages of 13 and 17, you represent that your legal guardian has
reviewed and agreed to these Terms.

You must provide all equipment and software necessary to connect to the Service, including, but not
limited to, a mobile device that is suitable to connect with and use the Service, in cases where the
Service offers a mobile component."
You are responsible for any fees, including internet connection or mobile fees that you incur when
accessing the Service.

When creating or updating an Account on the Service, you may be required to provide certain
personal information, which may include your name, birth date, e-mail address, and, in some cases,
payment information. You agree that you will supply accurate and complete information and that
you will update that information promptly after it changes.

You understand that your user ID number, name and profile picture will be publicly available and
that search engines may index your name and profile photo.

Any use of the Service in violation of these License Limitations is strictly prohibited, can result in the
immediate revocation of your limited license and may subject you to liability for violations of law.
any attempt by you to disrupt or interfere with the service including undermining or manipulating
the legitimate operation of any free game and top apps game is a violation of free games and top
apps term of use and may be a violation of criminal and civil laws.

Without limiting any other remedies, free games and top apps may limit, suspend, terminate,
modify, or delete accounts or access to the service or portions thereof if you are, or we suspects that
you are, failing to comply with any of these terms of use or for any actual or suspected illegal or
improper use of the service, with or without notice to you. you can lose your user name as a result
of account termination or limitation, as well as any benefits, privileges, earned items and purchased
items associated with your use of the service, and we are under no obligation to compensate you for
any such losses or results.

Without limiting our other remedies, we may limit, suspend or terminate the service and user
accounts or portions thereof, prohibit access to our games and sites, and their content, services and
tools, delay or remove hosted content, and take technical and legal steps to prevent users from
accessing the service if we believe that they are creating risk or possible legal liabilities, infringing
the intellectual property rights of third parties, or acting inconsistently with the letter or spirit of our
terms or policies. additionally, we may, in appropriate circumstances and at our sole discretion,
suspend or terminate accounts of users who may be repeat infringers of third party intellectual
property rights.

Free Games and Top Apps reserves the right to stop offering and/or supporting the Service or a
particular game or part of the Service at any time either permanently or temporarily, at which point
your license to use the Service or a part thereof will be automatically terminated or suspended. In
such event, Free Games and Top Apps shall not be required to provide refunds, benefits or other
compensation to users in connection with such discontinued elements of the Service.

Notwithstanding anything to the contrary herein, you acknowledge and agree that you shall have no
ownership or other property interest in an account, and you further acknowledge and agree that all
rights in and to an account are and shall forever be owned by and inure to the benefit of free games
and top apps. generally, game or other accounts created with free games and top apps will be
considered active until we receive a user request to deactivate or delete them; however, we reserve
the right to terminate any account that has been inactive for 180 days.

You have no right or title in or to any content that appears in the Service, including without
limitation the Virtual Items appearing or originating in any Free Games and Top Apps game, whether
Ã¬earnedÃ® in a game or Ã¬purchasedÃ® from any other attributes associated with an Account or stored
on the Service.

Free Games and Top Apps prohibits and does not recognize any purported transfers of Virtual Items
effectuated outside of the Service, or the purported sale, gift or trade in the \"real world\" of anything
that appears or originates in the Service, unless otherwise expressly authorized by Free Games and
Top Apps in writing. Accordingly, you may not sublicense, trade, sell or attempt to sell in-game
Virtual Items for \"real\" money, or exchange Virtual Items for value of any kind outside of a game,
without Free Game and Top AppsÃ­s written permission. Any such transfer or attempted transfer is
prohibited and void, and will subject your Account to termination.

You own your User Content. You hereby grant Free Game and Top Apps and its Affiliates a perpetual
and irrevocable, worldwide, fully paid-up and royalty free, non-exclusive, unlimited license, including
the right to sublicense and assign to third parties, and right to copy, reproduce, fix, adapt, modify,
improve, translate, reformat, create derivative works from, manufacture, introduce into circulation,
commercialize, publish, distribute, sell, license, sublicense, transfer, rent, lease, transmit, publicly
display, publicly perform, or provide access to electronically, broadcast, communicate to the public
by telecommunication, display, perform, enter into computer memory, and use and practice, in any
way now known or in the future discovered, your User Content as well as all modified and derivative
works thereof in connection with our provision of the Service, including marketing and promotions
thereof. To the extent permitted by applicable laws, you hereby waive any moral rights you may
have in any User Content. The license you grant Us to use user posted content (except any content
you submit in response to Free Game and Top Apps promotions and competitions or any other content specifically solicited by Free Game and Top Apps) ends when you delete your User Content or you close your Account unless your User Content has been shared with others, and they have not deleted it. However, you understand and accept that removed content may persist in back-up copies for a reasonable period of time.\n\nYou are entirely responsible for all User Content you post or otherwise transmit via the Service. Free Games and Top Apps assumes no responsibility for the conduct of any user submitting any User Content, and assumes no responsibility for monitoring the Service for inappropriate or illegal content or conduct.\n\nFree Games and Top Apps may reject, refuse to post or delete any User Content for any or no reason, including, but not limited to, User Content that in the sole judgment of Free Games and Top Apps may violate these Terms of Use.\n\nYour information, and the contents of all of your online communications (including without limitation chat text, voice communications, IP addresses and your personal information) may be accessed and monitored as necessary to provide the Service and may be disclosed: (i) when We have a good faith belief that We are required to disclose the information in response to legal process (for example, a court order, search warrant or subpoena); (ii) to satisfy any applicable laws or regulations (iii) where We believe that the Service is being used in the commission of a crime, including to report such criminal activity or to exchange information with other companies and organizations for the purposes of fraud protection and credit risk reduction; (iv) when We have a good faith belief that there is an emergency that poses a threat to the health and/or safety of you, another person or the public generally; and (v) in order to protect the rights or property of Free Games and Top Apps, including to enforce our Terms of Use. By entering into these Terms of Use, you hereby provide your irrevocable consent to such monitoring, access and disclosure.\n\nPurchases or redemptions of third party virtual currency to acquire a license to use virtual items are non-refundable.\n\nFree Games and Top Apps may provide links on the Service to third party websites or vendors who may invite you to participate in a promotional offer in return for receiving an optional component of the Service and/or upgrades (such as in-game currency). Any charges or obligations you incur in your dealings with these third parties are your responsibility. Free Games and Top Apps makes no representation or warranty regarding any content, goods and/or services provided by any third party even if linked to from our Service, and will not be liable for any claim relating to any third party content, goods and/or services. The linked sites are not under the control of Free Games and Top Apps and may collect data or solicit personal information from you. Free Games and Top Apps is not responsible for their content, business practices or privacy policies, or for the collection, use or disclosure of any information those sites may collect. Further, the inclusion of any link does not imply endorsement by Free Games and Top Apps of these linked sites.\n\nYou understand that the Service is an evolving one. Free Games and Top Apps may require that you accept updates to the Service and to Free Games and Top Apps games you have installed on your computer or mobile device. You acknowledge and agree that Free Games and Top Apps may update the Service with or without notifying you. You may need to update third party software from time to time in order to receive the Service and play Free Games and Top Apps Games.\n\nWithout limiting the foregoing, neither free games and top apps nor its affiliates or subsidiaries, or any of their directors, employees, agents, attorneys, third-party content providers, distributors, licensees or licensors (collectively, \" free games and top apps parties\") warrant that the service will be uninterrupted or error-free.\n\nFree Games and Top Apps shall not be liable for any delay or failure to perform resulting from causes outside the reasonable control of Free Games and Top Apps, including without limitation any failure to perform hereunder due to unforeseen circumstances or cause beyond Free Games and Top Apps control such as acts of God, war, terrorism, riots, embargoes, acts of civil or military authorities, fire, floods, accidents, network infrastructure failures, strikes, or shortages of transportation facilities, fuel, energy, labor or materials.
  */