using System;
using System.Linq;
using System.Text;
using InfinityScript;
using static InfinityScript.GSCFunctions;

public unsafe class CoD4 : BaseScript
{
    //private static int[] nvOffsets = new int[18];
    private int[] classOffsets = new int[10];
    private static HudElem[] scores = new HudElem[2];
    private static HudElem[] scoreBars = new HudElem[2];
    private static bool isTeamBased = true;
    public CoD4()
    {
        //nvOffsets[0] = 0x1AC246C;
        //for (int i = 1; i < 18; i++)
        //{
            //nvOffsets[i] = nvOffsets[i - 1] + 0x38AC;
        //}
        classOffsets[0] = 0x648111C;
        classOffsets[1] = 0x648117f;
        classOffsets[2] = 0x64811e2;
        classOffsets[3] = 0x6481245;
        classOffsets[4] = 0x64812a8;
        classOffsets[5] = 0x648136E;
        classOffsets[6] = 0x64813D1;
        classOffsets[7] = 0x6481434;
        classOffsets[8] = 0x6481497;
        classOffsets[9] = 0x64814fa;
        PreCacheShader("hud_grenadeicon");
        PreCacheShader("hud_flashbangicon");
        PreCacheShader("iw5_cardicon_elite_14");
        PreCacheShader("hud_icon_rpg_dpad");
        PreCacheShader("hud_icon_c4");
        PreCacheShader("hud_icon_claymore");
        PreCacheShader("faction_128_ussr");
        PreCacheShader("faction_128_af");
        PreCacheShader("faction_128_ic");
        PreCacheShader("faction_128_pmc");
        PreCacheShader("faction_128_sas");
        PreCacheShader("faction_128_delta");
        PreCacheShader("faction_128_gign");

        string gametype = GetDvar("g_gametype");
        if (gametype == "dm" || gametype == "gun" || gametype == "oic" || gametype == "jugg")
            isTeamBased = false;

        SetDvar("g_hardcore", "1");
        SetDvar("cg_drawCrosshair", "1");
        classOffsets[0] = classOffsets[0] + 0x1A;
        *(int*)classOffsets[0] = 0x01;
        classOffsets[0] = classOffsets[0] + 0x02;
        *(int*)classOffsets[0] = 0x8C;
        classOffsets[0] = classOffsets[0] + 0x02;
        *(int*)classOffsets[0] = 0x09;
        classOffsets[1] = classOffsets[1] + 0x1A;
        *(int*)classOffsets[1] = 0x65;
        classOffsets[1] = classOffsets[1] + 0x02;
        *(int*)classOffsets[1] = 0x46;
        classOffsets[1] = classOffsets[1] + 0x02;
        *(int*)classOffsets[1] = 0x0C;
        classOffsets[2] = classOffsets[2] + 0x1A;
        *(int*)classOffsets[2] = 0x70;
        classOffsets[2] = classOffsets[2] + 0x02;
        *(int*)classOffsets[2] = 0x12;
        classOffsets[2] = classOffsets[2] + 0x02;
        *(int*)classOffsets[2] = 0x84;
        classOffsets[3] = classOffsets[3] + 0x1A;
        *(int*)classOffsets[3] = 0x67;
        classOffsets[3] = classOffsets[3] + 0x02;
        *(int*)classOffsets[3] = 0x46;
        classOffsets[3] = classOffsets[3] + 0x02;
        *(int*)classOffsets[3] = 0x79;
        classOffsets[4] = classOffsets[4] + 0x1A;
        *(int*)classOffsets[4] = 0x6E;
        classOffsets[4] = classOffsets[4] + 0x02;
        *(int*)classOffsets[4] = 0x8C;
        classOffsets[4] = classOffsets[4] + 0x02;
        *(int*)classOffsets[4] = 0x84;

        classOffsets[5] = classOffsets[5] + 0x1A;
        *(int*)classOffsets[5] = 0x01;
        classOffsets[5] = classOffsets[5] + 0x02;
        *(int*)classOffsets[5] = 0x8C;
        classOffsets[5] = classOffsets[5] + 0x02;
        *(int*)classOffsets[5] = 0x09;
        classOffsets[6] = classOffsets[6] + 0x1A;
        *(int*)classOffsets[6] = 0x65;
        classOffsets[6] = classOffsets[6] + 0x02;
        *(int*)classOffsets[6] = 0x46;
        classOffsets[7] = classOffsets[7] + 0x1A;
        *(int*)classOffsets[7] = 0x70;
        classOffsets[7] = classOffsets[7] + 0x02;
        *(int*)classOffsets[7] = 0x12;
        classOffsets[7] = classOffsets[7] + 0x02;
        *(int*)classOffsets[7] = 0x84;
        classOffsets[8] = classOffsets[8] + 0x1A;
        *(int*)classOffsets[8] = 0x67;
        classOffsets[8] = classOffsets[8] + 0x02;
        *(int*)classOffsets[8] = 0x46;
        classOffsets[8] = classOffsets[8] + 0x02;
        *(int*)classOffsets[8] = 0x79;
        classOffsets[9] = classOffsets[9] + 0x1A;
        *(int*)classOffsets[9] = 0x6E;
        classOffsets[9] = classOffsets[9] + 0x02;
        *(int*)classOffsets[9] = 0x8C;
        classOffsets[9] = classOffsets[9] + 0x02;
        *(int*)classOffsets[9] = 0x84;

        createServerHud();

        PlayerConnected += onPlayerConnected;
    }

    private static void onPlayerConnected(Entity player)
    {
        player.SetClientDvar("g_hardcore", "1");
        //entity.SetClientDvar("g_compassShowEnemies", "1");

        createHud(player);

        player.NotifyOnPlayerCommand("tactical_thrown", "+smoke");
        player.OnNotify("tactical_thrown", (p) =>
        {
            if (player.GetAmmoCount("concussion_grenade_mp") > 0 && player.HasWeapon("concussion_grenade_mp"))
            {
                player.SetPerk("specialty_fastoffhand", true, true);
                AfterDelay(1000, () =>
                    player.UnSetPerk("specialty_fastoffhand", true));
            }
        });

        player.OnNotify("weapon_change", (p, newWeap) =>
        {
            if (mayDropWeapon((string)newWeap))
                player.SetField("lastDroppableWeapon", (string)newWeap);
            if (player.CurrentWeapon == "c4_mp" || player.CurrentWeapon == "claymore_mp" || player.CurrentWeapon == "rpg_mp")
                player.DisableWeaponPickup();
            else player.EnableWeaponPickup();
            killstreakSwitcher(player);
            updateHUDAmmo(player);

            HudElem equipTXT = player.GetField<HudElem>("hud_equipTXT");
            HudElem equipIcon = player.GetField<HudElem>("hud_equip");
            HudElem equipAmmo = player.GetField<HudElem>("hud_equipAmmo");
            if (!player.HasWeapon("rpg_mp") && !player.HasWeapon("claymore_mp") && !player.HasWeapon("c4_mp"))
            {
                equipTXT.Alpha = 0;
                equipIcon.Alpha = 0;
                equipAmmo.Alpha = 0;
                return;
            }
            if ((string)newWeap != "c4_mp" && (string)newWeap != "claymore_mp" && (string)newWeap != "rpg_mp")
            {
                equipIcon.Alpha = 0.5f;
                equipAmmo.Alpha = 1;
                equipTXT.Alpha = 1;
            }
            else if ((string)newWeap == "c4_mp" || (string)newWeap == "claymore_mp" || (string)newWeap == "rpg_mp")
                equipIcon.Alpha = 1f;
        });

        player.OnNotify("weapon_fired", (p, weapon) =>
            updateHUDAmmo(player));

        player.OnNotify("reload", updateHUDAmmo);

        player.OnNotify("grenade_fire", (p, weapon, grenade) =>
            updateHUDAmmo(player));

        player.SpawnedPlayer += new Action(() =>
        {
            player.SetClientDvar("g_hardcore", "1");
            //entity.SetClientDvar("g_compassShowEnemies", "1");

            player.SetPerk("specialty_bulletdamage", true, true);//Nightvision

            // update all players' ranking
            updateScores();
            handleEquipment(player);

            updateHUDAmmo(player);
        });
    }

    private static bool mayDropWeapon(string weapon)
    {
        if (weapon == "none")
            return false;

        if (weapon.Contains("ac130"))
            return false;

        if (weapon.Contains("rpg_mp"))
            return false;

        string invType = WeaponInventoryType(weapon);
        if (invType != "primary")
            return false;

        return true;
    }

    public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
    {
        // update all players' ranking
        AfterDelay(100, updateScores);
        if (player.CurrentWeapon == "rpg_mp")
            player.TakeWeapon("rpg_mp");
    }

    private static void updateScores()
    {
        if (isTeamBased) updateScores_teamBased();
        else updateScores_ffa();
    }
    private static void updateScores_teamBased()
    {
        float alliesScore = GetTeamScore("allies");
        float axisScore = GetTeamScore("axis");
        string gametype = GetDvar("g_gametype");
        float scoreLimit = GetDvarInt("scr_" + gametype + "_scorelimit");
        scores[0].SetValue((int)alliesScore);
        scores[1].SetValue((int)axisScore);
        float alliesBarScale = alliesScore / scoreLimit;
        float axisBarScale = axisScore / scoreLimit;
        scoreBars[0].SetShader("white", (int)(100 * alliesBarScale) + 2, 5);
        scoreBars[1].SetShader("white", (int)(100 * axisBarScale) + 2, 5);
    }
    private static void updateScores_ffa()
    {
        var scoreList = (from p in Players
                         orderby p.Score descending, p.Deaths ascending
                         select p).ToList();

        scores[0].SetValue(scoreList[0].Score);
        if (Players.Count > 1) scores[1].SetValue(scoreList[1].Score);
        else scores[1].SetValue(0);
    }

    public static string createHudShaderString(string shader, bool flipped = false, int width = 64, int height = 64)
    {
        byte[] str;
        byte flip;
        flip = (byte)(flipped ? 2 : 1);
        byte w = (byte)width;
        byte h = (byte)height;
        byte length = (byte)shader.Length;
        str = new byte[4] { flip, w, h, length };
        string ret = "^" + Encoding.UTF8.GetString(str);
        return ret + shader;
    }
    private static void createServerHud()
    {
        HudElem divider = NewHudElem();
        divider.X = -10;
        divider.Y = 67;
        divider.AlignX = HudElem.XAlignments.Left;
        divider.AlignY = HudElem.YAlignments.Bottom;
        divider.HorzAlign = HudElem.HorzAlignments.Left;
        divider.VertAlign = HudElem.VertAlignments.Bottom;
        divider.Alpha = 1;
        divider.HideWhenInMenu = true;
        divider.Foreground = false;
        divider.LowResBackground = true;
        divider.Sort = 4;
        divider.Font = HudElem.Fonts.HudBig;
        divider.FontScale = 6;
        //divider.SetShader("hud_iw5_divider", -200, 24);
        divider.SetText(createHudShaderString("hud_iw5_divider", true, 255, 24));

        for (int i = 0; i < 2; i++)
        {
            var bar = NewHudElem();
            bar.X = 45;
            if (i == 0) bar.Y = 7;
            else bar.Y = -2;
            bar.AlignX = HudElem.XAlignments.Left;
            bar.AlignY = HudElem.YAlignments.Bottom;
            bar.HorzAlign = HudElem.HorzAlignments.Left;
            bar.VertAlign = HudElem.VertAlignments.Bottom;
            bar.SetShader("white", 2, 5);
            bar.Alpha = .9f;
            bar.HideWhenInMenu = true;
            bar.Foreground = false;
            if (i == 0) bar.Color = new Vector3(1, .2f, .2f);
            else bar.Color = new Vector3(.2f, 1, .2f);

            scoreBars[i] = bar;
        }

        for (int i = 0; i < 2; i++)
        {
            var text = NewHudElem();
            text.X = 30;
            if (i == 0) text.Y = 14;
            else text.Y = 0;
            text.AlignX = HudElem.XAlignments.Left;
            text.AlignY = HudElem.YAlignments.Bottom;
            text.HorzAlign = HudElem.HorzAlignments.Left;
            text.VertAlign = HudElem.VertAlignments.Bottom;
            text.FontScale = 1f;
            text.Font = HudElem.Fonts.HudSmall;
            text.Alpha = 1f;
            text.HideWhenInMenu = true;
            text.Sort = 5;
            text.SetValue(0);

            scores[i] = text;
        }

        for (string s = "Allies"; ; s = "Axis")
        {
            var icon = NewTeamHudElem(s.ToLower());
            icon.X = -40;
            icon.Y = 24;
            icon.AlignX = HudElem.XAlignments.Left;
            icon.AlignY = HudElem.YAlignments.Bottom;
            icon.HorzAlign = HudElem.HorzAlignments.Left;
            icon.VertAlign = HudElem.VertAlignments.Bottom;
            icon.FontScale = 5;
            icon.Sort = 3;
            icon.HideWhenInMenu = true;
            string shader = GetDvar("g_teamIcon_" + s);
            icon.SetShader(shader, 48, 48);

            if (s == "Axis") break;
        }
    }

    private static void createHud(Entity player)
    {
        if (player.HasField("cod4hud_created"))
            return;

        // ammo stuff
        var ammoSlash = HudElem.CreateFontString(player, HudElem.Fonts.Objective, 1.5f);
        ammoSlash.SetPoint("bottom right", "bottom right", -85, -15);
        ammoSlash.GlowAlpha = 0;
        ammoSlash.HideWhenInMenu = true;
        ammoSlash.Archived = true;
        ammoSlash.SetText("/");

        player.SetField("hud_ammoSlash", new Parameter(ammoSlash));

        var ammoStock = HudElem.CreateFontString(player, HudElem.Fonts.Objective, 1.5f);
        ammoStock.Parent = ammoSlash;
        ammoStock.SetPoint("bottom left", "bottom left", 3, 0);
        ammoStock.GlowAlpha = 0;
        ammoStock.HideWhenInMenu = true;
        ammoStock.Archived = true;
        ammoStock.SetValue(48);

        player.SetField("hud_ammoStock", new Parameter(ammoStock));

        var ammoClip = HudElem.CreateFontString(player, HudElem.Fonts.Objective, 2f);
        ammoClip.Parent = ammoSlash;
        ammoClip.SetPoint("right", "right", -7, -0);
        ammoClip.GlowAlpha = 0;
        ammoClip.HideWhenInMenu = true;
        ammoClip.Archived = true;
        ammoClip.SetValue(12);

        var weaponName = HudElem.CreateFontString(player, HudElem.Fonts.Objective, 1.5f);
        weaponName.SetPoint("bottom right", "bottom right", -110, -35);
        weaponName.GlowAlpha = 0;
        weaponName.HideWhenInMenu = true;
        weaponName.Archived = true;
        weaponName.SetText("");

        var flash = HudElem.CreateFontString(player, HudElem.Fonts.Objective, 1.4f);
        flash.SetPoint("bottom right", "bottom right", -80, -32);
        flash.GlowAlpha = 0;
        flash.HideWhenInMenu = true;
        flash.Archived = true;
        flash.SetValue(0);
        flash.Color = new Vector3(1, 0, 0);

        var grenade = HudElem.CreateFontString(player, HudElem.Fonts.Objective, 1.4f);
        grenade.SetPoint("bottom right", "bottom right", -50, -32);
        grenade.GlowAlpha = 0;
        grenade.HideWhenInMenu = true;
        grenade.Archived = true;
        grenade.SetValue(0);
        grenade.Color = new Vector3(1, 0, 0);

        HudElem grenadeIcon = HudElem.CreateIcon(player, "hud_grenadeicon", 15, 15);
        grenadeIcon.SetPoint("BOTTOMRIGHT", "BOTTOMRIGHT", -60, -35);
        grenadeIcon.HideWhenInMenu = true;
        grenadeIcon.Foreground = true;
        grenadeIcon.Archived = true;
        //grenadeIcon.SetShader("hud_grenadeicon", 15, 15);
        grenadeIcon.Alpha = 0.7f;

        HudElem flashIcon = HudElem.CreateIcon(player, "hud_flashbangicon", 15, 15);
        flashIcon.SetPoint("BOTTOMRIGHT", "BOTTOMRIGHT", -85, -35);
        flashIcon.HideWhenInMenu = true;
        flashIcon.Foreground = true;
        flashIcon.Archived = true;
        //flashIcon.SetShader("hud_flashbangicon", 15, 15);
        flashIcon.Alpha = 1;

        HudElem nvIcon = HudElem.CreateIcon(player, "iw5_cardicon_elite_14", 20, 20);
        nvIcon.SetPoint("BOTTOM", "BOTTOM", 0, -15);
        nvIcon.HideWhenInMenu = true;
        nvIcon.Foreground = true;
        nvIcon.Archived = true;
        //nvIcon.SetShader("iw5_cardicon_elite_14", 20, 20);
        nvIcon.Alpha = 1;

        var nvHint = HudElem.CreateFontString(player, HudElem.Fonts.Objective, 1f);
        nvHint.SetPoint("bottom", "bottom", 0, -5);
        nvHint.GlowAlpha = 0;
        nvHint.HideWhenInMenu = true;
        nvHint.Archived = true;
        nvHint.SetText("[[{+actionslot 3}]]");

        /*
        player.NotifyOnPlayerCommand("nightvision", "+actionslot 4");
        player.SetField("isNight", 0);
        player.OnNotify("nightvision", (ent) =>
        {
            if (player.GetField<int>("isNight") == 0)
            {
                *(int*)nvOffsets[player.EntRef] = 64;
                player.PlayLocalSound("item_nightvision_on");
                player.SetField("isNight", 1);
            }
            else
            {
                *(int*)nvOffsets[player.EntRef] = 0;
                player.PlayLocalSound("item_nightvision_off");
                player.SetField("isNight", 0);
            }
        });
        */

        HudElem equipIcon = HudElem.CreateIcon(player, "", 20, 20);
        equipIcon.SetPoint("BOTTOM", "BOTTOM", -80, -15);
        equipIcon.HideWhenInMenu = true;
        equipIcon.Foreground = true;
        equipIcon.Archived = true;
        equipIcon.Alpha = 0.5f;

        var equipHint = HudElem.CreateFontString(player, HudElem.Fonts.Objective, 1f);
        equipHint.SetPoint("bottom", "bottom", -80, -5);
        equipHint.GlowAlpha = 0;
        equipHint.HideWhenInMenu = true;
        equipHint.Archived = true;

        var equipAmmo = HudElem.CreateFontString(player, HudElem.Fonts.Objective, 1.2f);
        equipAmmo.SetPoint("bottom", "bottom", -70, -23);
        equipAmmo.GlowAlpha = 0;
        equipAmmo.HideWhenInMenu = true;
        equipAmmo.Archived = true;
        equipAmmo.SetText("");

        player.NotifyOnPlayerCommand("equipUse", "vote yes");

        player.SetField("hud_weaponName", new Parameter(weaponName));

        player.SetField("hud_ammoClip", new Parameter(ammoClip));

        player.SetField("hud_slash", new Parameter(ammoSlash));

        player.SetField("hud_flash", new Parameter(flash));

        player.SetField("hud_grenade", new Parameter(grenade));

        player.SetField("hud_equip", new Parameter(equipIcon));

        player.SetField("hud_equipTXT", new Parameter(equipHint));

        player.SetField("hud_equipAmmo", new Parameter(equipAmmo));

        player.SetField("cod4hud_created", true);

        updateHUDAmmo(player);
    }

    private static void updateHUDAmmo(Entity player)
    {
        if (!player.HasField("cod4hud_created"))
            return;

        if (!player.IsAlive)
            return;

        HudElem ammoStock = player.GetField<HudElem>("hud_ammoStock");
        HudElem ammoClip = player.GetField<HudElem>("hud_ammoClip");
        HudElem ammoSlash = player.GetField<HudElem>("hud_slash");
        HudElem flash = player.GetField<HudElem>("hud_flash");
        HudElem grenade = player.GetField<HudElem>("hud_grenade");
        HudElem weaponName = player.GetField<HudElem>("hud_weaponName");
        HudElem equipAmmo = player.GetField<HudElem>("hud_equipAmmo");
        string currentWeapon = player.CurrentWeapon;
        int flashAmmo = 0;
        int equipAmmoCount = 0;

        if (currentWeapon == "c4_mp" || currentWeapon == "claymore_mp")
        {
            ammoStock.Alpha = 0;
            ammoClip.Alpha = 0; ;
            ammoSlash.SetText("");
        }
        else
        {
            ammoStock.Alpha = 1;
            ammoClip.Alpha = 1;
            ammoStock.SetValue(player.GetWeaponAmmoStock(currentWeapon));
            ammoClip.SetValue(player.GetWeaponAmmoClip(currentWeapon));
            ammoSlash.SetText("/");
        }
        if (player.HasWeapon("concussion_grenade_mp"))
            flashAmmo = player.GetAmmoCount("concussion_grenade_mp");
        else if (player.HasWeapon("smoke_grenade_mp"))
            flashAmmo = player.GetAmmoCount("smoke_grenade_mp");
        else
            flashAmmo = player.GetAmmoCount("flash_grenade_mp");

        if (flashAmmo == 0)
        {
            flash.SetValue(flashAmmo);
            flash.Color = new Vector3(1, 0, 0);
        }
        else
        {
            flash.SetValue(flashAmmo);
            flash.Color = new Vector3(1, 1, 1);
        }

        int fragAmmo = player.GetAmmoCount("frag_grenade_mp");
        grenade.SetValue(fragAmmo);
        if (fragAmmo == 0) grenade.Color = new Vector3(1, 0, 0);
        else grenade.Color = new Vector3(1, 1, 1);

        if (player.HasWeapon("c4_mp"))
            equipAmmoCount = player.GetAmmoCount("c4_mp");
        else if (player.HasWeapon("claymore_mp"))
            equipAmmoCount = player.GetAmmoCount("claymore_mp");
        else if (player.HasWeapon("rpg_mp"))
            equipAmmoCount = player.GetAmmoCount("rpg_mp");

        equipAmmo.SetValue(equipAmmoCount);
        if (equipAmmoCount == 0)
            equipAmmo.Color = new Vector3(1, 0, 0);
        else equipAmmo.Color = new Vector3(1, 1, 1);

        var weapon = player.CurrentWeapon;

        if (weapon.Contains("iw5_usp45_"))
            weaponName.SetText("USP.45");
        else if (weapon.Contains("iw5_deserteagle_"))
            weaponName.SetText("Desert Eagle");
        else if (weapon.Contains("iw5_m4_"))
            weaponName.SetText("M4 Carbine");
        else if (weapon.Contains("iw5_ak47_"))
            weaponName.SetText("AK-47");
        else if (weapon.Contains("iw5_g36c_"))
            weaponName.SetText("G36C");
        else if (weapon.Contains("iw5_m16_"))
            weaponName.SetText("M16A4");
        else if (weapon.Contains("iw5_mk14_"))
            weaponName.SetText("M14");
        else if (weapon.Contains("iw5_mp5_"))
            weaponName.SetText("MP5");
        else if (weapon.Contains("iw5_m9_"))
            weaponName.SetText("Mini-Uzi");
        else if (weapon.Contains("iw5_p90_"))
            weaponName.SetText("P90");
        else if (weapon.Contains("iw5_skorpion_"))
            weaponName.SetText("Skorpion");
        else if (weapon.Contains("iw5_usas12_"))
            weaponName.SetText("M1014");
        else if (weapon.Contains("iw5_ksg_"))
            weaponName.SetText("W1200");
        else if (weapon.Contains("iw5_m60_"))
            weaponName.SetText("M60E4");
        else if (weapon.Contains("iw5_barrett_"))
            weaponName.SetText("Barrett 50.cal");
        else if (weapon == "rpg_mp")
            weaponName.SetText("RPG-7");
        else if (weapon.Contains("iw5_msr_"))
            weaponName.SetText("M40A3");
        else if (weapon.Contains("iw5_dragunov_"))
            weaponName.SetText("Dragunov");
        else if (weapon.Contains("iw5_mk46_"))
            weaponName.SetText("M249 SAW");
        else if (weapon.Contains("c4_mp"))
            weaponName.SetText("C4 Detonator");
        else if (weapon.Contains("claymore_mp"))
            weaponName.SetText("Claymore");
        else weaponName.SetText("");
    }

    private static void killstreakSwitcher(Entity player)
    {
        string weapon = player.CurrentWeapon;
        if (weapon.Contains("killstreak_") && !weapon.Contains("airstrike") && !weapon.Contains("predator_missile"))
        {
            player.GiveWeapon("bomb_site_mp");
            AfterDelay(100, () =>
            player.SwitchToWeaponImmediate("bomb_site_mp"));
            AfterDelay(400, () =>
              player.SwitchToWeapon(player.GetField<string>("lastDroppableWeapon")));
            AfterDelay(800, () =>
            {
                player.TakeWeapon("bomb_site_mp");
            });
        }
    }
    private static void handleEquipment(Entity player)
    {
        HudElem equipIcon = player.GetField<HudElem>("hud_equip");
        HudElem equipTXT = player.GetField<HudElem>("hud_equipTXT");

        if (player.HasWeapon("c4_mp"))
        {
            player.TakeWeapon("c4_mp");
            player.SetOffhandPrimaryClass("frag");
            player.GiveWeapon("frag_grenade_mp");
            player.GiveWeapon("c4_mp");
            player.SetWeaponAmmoClip("c4_mp", 2);
            player.SetWeaponAmmoStock("c4_mp", 2);
            equipIcon.SetShader("hud_icon_c4", 20, 20);
            equipTXT.SetText("[[{vote yes}]]");
            player.OnNotify("equipUse", (p) =>
                {
                    if (player.CurrentWeapon != "c4_mp")
                    {
                        player.SwitchToWeapon("c4_mp");
                        equipIcon.Alpha = 1;
                    }
                    else
                    {
                        player.SwitchToWeapon(player.GetField<string>("lastDroppableWeapon"));
                    }
                });
        }
        else if (player.HasWeapon("claymore_mp"))
        {
            player.TakeWeapon("claymore_mp");
            player.SetOffhandPrimaryClass("frag");
            player.GiveWeapon("frag_grenade_mp");
            player.GiveWeapon("claymore_mp");
            player.SetWeaponAmmoClip("claymore_mp", 2);
            player.SetWeaponAmmoStock("claymore_mp", 2);
            equipIcon.SetShader("hud_icon_claymore", 20, 20);
            equipTXT.SetText("[[{vote yes}]]");
            player.OnNotify("weapon_fired", (ent, weapon) =>
                {
                    if ((string)weapon == "claymore_mp")
                    {
                        if (player.GetWeaponAmmoClip("claymore_mp") == 0)
                        {
                            equipIcon.SetShader("", 20, 20);
                            equipTXT.SetText("");
                        }
                    }
                });
            player.OnNotify("equipUse", (ent) =>
            {
                if (player.CurrentWeapon != "claymore_mp")
                {
                    player.SwitchToWeapon("claymore_mp");
                    equipIcon.Alpha = 1;
                }
                else
                {
                    player.SwitchToWeapon(player.GetField<string>("lastDroppableWeapon"));
                }
            });
        }
        else if (player.HasWeapon("rpg_mp"))
        {
            player.GiveWeapon("iw5_usp45_mp");
            equipIcon.SetShader("hud_icon_rpg_dpad", 20, 20);
            equipTXT.SetText("[[{vote yes}]]");
            player.OnNotify("weapon_fired", (ent, weap) =>
                {
                    if ((string)weap == "rpg_mp")
                    {
                        if (player.GetAmmoCount("rpg_mp") == 0)
                        {
                            AfterDelay(1000, () =>
                                player.TakeWeapon("rpg_mp"));
                            equipIcon.SetShader("", 20, 20);
                            equipTXT.SetText("");
                        }
                    }
                });
            player.OnNotify("equipUse", (ent) =>
            {
                if (player.CurrentWeapon != "rpg_mp")
                {
                    player.SwitchToWeapon("rpg_mp");
                    equipIcon.Alpha = 1;
                }
                else
                {
                    player.SwitchToWeapon(player.GetField<string>("lastDroppableWeapon"));
                }
            });
        }
        else if (!player.HasWeapon("rpg_mp") && !player.HasWeapon("claymore_mp") && !player.HasWeapon("c4_mp"))
        {
            equipIcon.SetShader("", 20, 20);
            equipTXT.Alpha = 0;
        }
    }
}