using UnityEngine;
using System.Collections;

public abstract class TDEnemy : TDActor {

	public enum Type
	{
		eImp = 0,
		eGargoyle  = 1
	};

	public delegate void EventHandler(TDEnemy enemy);
	public event EventHandler OnEventDestroy;

	// Use this for initialization
	protected override void Start () {
		base.Start();
		GameObject enemyHealthPrefab = (GameObject) Resources.Load("EnemyHealthBarPrefab");
		m_healthBar = (GameObject) Instantiate(enemyHealthPrefab, new Vector3(0.5f, 0.5f), new Quaternion());
		updateHealthBar();
		
		TDWorld world = TDWorld.getWorld();
		GameObject player = world.getPlayer();
		hasPathTo(player);

		m_state = State.eRunToPlayer;

		m_timer = Time.time;
	}

	// Update is called once per frame
	protected override void Update () {
		base.Update();		
		updateHealthBar();

		switch (m_state)
		{
			case State.eRunToPlayer:
				runToPlayer();
				break;
			case State.eFightHero:
				fightHero();
				break;
		}

	}

	void runToPlayer()
	{
		TDWorld world = TDWorld.getWorld();
		if ((null == m_path) || (Time.time - m_timer > world.m_configuration.enemyRecalcPathTime))
		{
			GameObject player = world.getPlayer();
			m_timer = Time.time;
			hasPathTo(player);
		}
		TDHero hero = world.getTDHero();
		if (hero.isAlive())
			if ((hero.transform.position - transform.position).magnitude < heroHostileRadius())
			{
				if (Random.value < heroHostileChance())
				{
					cleanPath();
					m_state = State.eFightHero;
				}
			}
	}

	void fightHero()
	{
		TDWorld world = TDWorld.getWorld();
		TDHero hero = world.getTDHero();
		if (!hero.isAlive())
		{
			m_state = State.eRunToPlayer;
			cleanPath();
			return;
		}
		if ((hero.transform.position - transform.position).magnitude < fightRadius())
		{
			cleanPath();
			TDDamage damage = new TDDamage(TDDamage.Type.ePhysical, physicalDamage()*Time.deltaTime, 0.0f); // Replace by override TDEnemy::getDamage
			hero.receiveDamage(damage, this);
		}
		else
		{
			if ((null == m_path) || (Time.time - m_timer > world.m_configuration.enemyRecalcPathTime))
			{
				m_timer = Time.time;
				hasPathTo(hero.gameObject);
			}
		}
		
	}

	override public void receiveDamage(TDDamage damage, TDActor source)
	{
		base.receiveDamage(damage, source);
		if (null != source)
		{
			TDWorld world = TDWorld.getWorld();
			TDHero hero = world.getTDHero();
			if (source == hero)
			{
				m_state = State.eFightHero;
			}
		}
	}

	void updateHealthBar()
	{
		if (m_healthBar == null)
			return;
		Vector3 txtPos = Camera.main.WorldToViewportPoint(transform.position);
		m_healthBar.transform.position = txtPos;
		float hpLeft = ((float) m_HP)/((float) getStartHP());
		hpLeft *= 30;
		m_healthBar.guiTexture.pixelInset = new Rect(0, 0, hpLeft, 10);
	}

	void OnDestroy()
	{
		Destroy(m_healthBar);
		if (OnEventDestroy != null)
			OnEventDestroy(this);
		TDWorld world = TDWorld.getWorld();
		if (world == null)
			return;
		TDPlayer player = world.getTDPlayer();
		if (player == null)
			return;
		player.reward(killReward());
	}

	protected override void onTargetReached(GameObject obj)
	{
		TDWorld world = TDWorld.getWorld();
		GameObject player = world.getPlayer();
		if (obj == player)
		{
			TDPlayer tdP = world.getTDPlayer();
			tdP.receiveDamage(1);
		}
		DestroyObject(gameObject);
	}

	protected override void onTargetDestroyed()
	{
	}

	protected abstract uint killReward();

	protected abstract float heroHostileChance();
	protected abstract float heroHostileRadius();
	protected abstract float fightRadius();
	protected abstract float physicalDamage();

	enum State
	{
		eRunToPlayer     = 0,
		eFightHero       = 1
	}
	State m_state;

	GameObject m_healthBar;
	float m_timer;
}
