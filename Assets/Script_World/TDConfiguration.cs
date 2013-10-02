using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class TDConfiguration
{
	public TDConfiguration()
	{
		drawGrid = 1;
		drawPath = 1;
		gridNbCellsX = 20;
		gridNbCellsY = 20;

		hitDistance = 0.3f;

		playerHP             = 10;

	}

	public void readFromResource()
	{
		Dictionary<string, string> gameConf = getLines();

		System.Type type = typeof(TDConfiguration);
		type.GetFields();
		FieldInfo[] fields = type.GetFields();
		foreach (var field in fields)
		{
		    string name = field.Name;
			if (!gameConf.ContainsKey(name))
				continue;
			
		    object val = field.GetValue(this);
		    if (val is uint) 
		    {
				uint x = Convert.ToUInt32(gameConf[name]);
				field.SetValue(this, x);
			}
		    else if (val is float)
		    {
				float x = Convert.ToSingle(gameConf[name]);
				field.SetValue(this, x);
			}
		}
	}

	public Dictionary<string, string> getLines()
	{
		Dictionary<string, string> dic = new Dictionary<string, string>();
		TextAsset textFile = (TextAsset)Resources.Load("Configuration", typeof(TextAsset));
		if (textFile == null)
			return dic;
        System.IO.StringReader textStream = new System.IO.StringReader(textFile.text);
   		string line;
        while ((line = textStream.ReadLine()) != null)
		{
			// Skip comment
			if (line.Length < 2)
				continue;
			if (line.Contains("//"))
				continue;
			string [] aToken = line.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries );
			if (aToken.Length < 3)
				continue;
			dic[aToken[0]] = aToken[2];
		}
		return dic;
	}

	public uint drawGrid; // Draws grid over terrain
	public uint drawPath; // Draws paths for enemies
	public uint gridNbCellsX;
	public uint gridNbCellsY;

	public float hitDistance;

	public uint nbTrees;

	public uint playerHP;
	public uint playerMoney;

	public uint  heroHP;
	public float heroSpeed;
	public float heroFightRadius;
	public float heroPatrolRadius;
	public float heroPhysicalDamagePerSec;
	public float heroAutoHealPerSec;
	public float heroDeathTime;

	public float enemyRecalcPathTime;

	public uint enemyImpHP;
	public uint enemyImpKillReward;
	public float enemyImpSpeed;
	public float enemyImpHeroHostileChance;
	public float enemyImpHeroHostileRadius;
	public float enemyImpFightRadius;
	public float enemyImpPhysicalDamagePerSec;

	public uint enemyGargoyleHP;
	public uint enemyGargoyleKillReward;
	public float enemyGargoyleSpeed;
	public float enemyGargoyleHeroHostileChance;
	public float enemyGargoyleHeroHostileRadius;
	public float enemyGargoyleFightRadius;
	public float enemyGargoylePhysicalDamagePerSec;
	
	//Towers
	public float towerArcherPhysicalDamage;
	public float towerArcherProjectileSpeed;
	public float towerArcherRestoration;
	public float towerArcherRadius;
	public uint  towerArcherPrice;
	
	public float towerCanonPhysicalDamage;
	public float towerCanonProjectileSpeed;
	public float towerCanonBallDamageRadius;
	public float towerCanonBallParabolaHeight;
	public float towerCanonRestoration;
	public float towerCanonRadius;
	public uint  towerCanonPrice;
}
