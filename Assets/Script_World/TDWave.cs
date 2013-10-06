using UnityEngine;
using System.Collections;

public class TDWave 
{
	public void start()
	{
		m_startTime = Time.time;
		m_watchedEnemies = 0;
	}

	public void init(string [] aEnemiesScenario)
	{
		m_aEnemiesScenario = aEnemiesScenario;
		m_maxStringLength = 0;
		for (int i=0; i<m_aEnemiesScenario.Length; i++)
		{
			if (m_maxStringLength < m_aEnemiesScenario[i].Length)
				m_maxStringLength = (uint) m_aEnemiesScenario[i].Length;
		}
		m_currentPosition = 0;
	}

	public bool isFinished()
	{
		return (m_currentPosition >= m_maxStringLength);
	}
	public bool isNextEnemyReady()
	{
		if (isFinished())
			return false;
		if (m_startTime + maxTimeBetweenEnemies() < Time.time)
			return true;
		return (Random.value < chanceOfEarlyAppearance());
	}
	public void launchEnemies()
	{
		m_startTime = Time.time;
		TDWorld world = TDWorld.getWorld();
		for (uint i=0; i<m_aEnemiesScenario.Length; i++)
		{
			string curString = m_aEnemiesScenario[i];
			if (m_currentPosition >= curString.Length)
				continue;
			Vector3 respawnPosition = world.getRespawnPoint(i).transform.position;
			char c = curString[(int)m_currentPosition];
			GameObject newEnemy = null;
			switch (c)
			{
				case 'I':
					newEnemy = world.addEnemy3d(TDEnemy.Type.eImp, respawnPosition);
					break;
				case 'G':
					newEnemy = world.addEnemy3d(TDEnemy.Type.eGargoyle, respawnPosition);
					break;
			}
			if (null != newEnemy)
			{
				m_watchedEnemies++;
				TDEnemy tdEnemy = world.getTDEnemy(newEnemy);
				tdEnemy.OnEventDestroy += destroyCallback;
			}
		}
		m_currentPosition++;
	}

	public void destroyCallback(TDEnemy enemy)
	{
		enemy.OnEventDestroy -= destroyCallback;
		m_watchedEnemies--;
	}

	public float maxTimeBetweenEnemies()
	{
		return 0.5f;
	}
	public float chanceOfEarlyAppearance()
	{
		return 0.01f;
	}

	public bool allEnemiesAreKilled()
	{
		if (m_currentPosition < m_maxStringLength)
			return false;
		return (m_watchedEnemies <= 0);
	}
	
	int m_watchedEnemies;
	uint m_maxStringLength;
	uint m_currentPosition;
	string [] m_aEnemiesScenario;	
	float m_startTime;
}

public class TDWaves
{
	public TDWaves()
	{
		m_currentWaveIndex = 0;
		m_intervalBetweenWaves = 10.0f;
	}

	string [] getInitStrings(int index)
	{
		string [] aEnemyScenario = new string[2];
		switch (index)
		{
			case 0:
				aEnemyScenario[0] = "III";
				aEnemyScenario[1] = "II";
				break;
			case 1:
				aEnemyScenario[0] = "IIII";
				aEnemyScenario[1] = "II";
				break;
			case 2:
				aEnemyScenario[0] = "IIIIII";
				aEnemyScenario[1] = "IIII";
				break;
			case 3:
				aEnemyScenario[0] = "IIIIII";
				aEnemyScenario[1] = "GGG";
				break;
			case 4:
				aEnemyScenario[0] = "IIIIII";
				aEnemyScenario[1] = "IIIGGG";
				break;
			case 5:
				aEnemyScenario[0] = "IIIGIGIGGGG";
				aEnemyScenario[1] = "IIGGGGGG";
				break;
			case 6:
				aEnemyScenario[0] = "GGGGGGGGG";
				aEnemyScenario[1] = "GGGGGGGGGG";
				break;
			case 7:
				aEnemyScenario[0] = "IIIIIIGGGGIII";
				aEnemyScenario[1] = "IIIIGGGGG";
				break;
			case 8:
				aEnemyScenario[0] = "IIIIIIGGGGGGGIIIIIGGGGIIII";
				aEnemyScenario[1] = "GGGIIIIIGIGIGIGIGIGIGG";
				break;
			case 9:
				aEnemyScenario[0] = "IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII";
				aEnemyScenario[1] = "IIIGGGGGGGGGGGGGGGGGGIIIIIIIIIIIGGGGGGGGGGGGGGGG";
				break;
		}
		return aEnemyScenario;
	}

	public void readFromResource()
	{
		m_aWaves = new TDWave[10];
		for (int i=0; i<m_aWaves.Length; i++)
		{
			m_aWaves[i] = new TDWave();
			TDWave wave = m_aWaves[i];
			string [] aStr = getInitStrings(i);
			wave.init(aStr);
		}
	}

	public enum State
	{
		eWave,
		eFinished
	}
	public float intervalBetweenWaves()
	{
		return m_intervalBetweenWaves;
	}

	public State getState()
	{
		if (m_currentWaveIndex >= m_aWaves.Length)
			return State.eFinished;
		return State.eWave;
	}

	public TDWave getCurrentWave()
	{
		if (m_currentWaveIndex >= m_aWaves.Length)
			return null;
		return m_aWaves[m_currentWaveIndex];
	}

	public void switchToNextWave()
	{
		m_currentWaveIndex++;
		TDWave wave = getCurrentWave();
		if (null == wave)
			return;
		wave.start();
	}
	
	float m_intervalBetweenWaves;
	uint m_currentWaveIndex;
	TDWave [] m_aWaves;
}
