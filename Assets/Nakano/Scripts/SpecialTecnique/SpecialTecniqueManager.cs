using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialTecniqueManager : MonoBehaviour
{
    public SpecialTecnique[] specialTecniques;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ReleaseInitialize()
    {
        for (int i = 0; i < specialTecniques.Length; i++)
        {
            specialTecniques[i].m_released = false;
        }
    }

    /// <summary>
    /// ����Z�\�̃f�[�^���擾�E�ݒ�
    /// </summary>
    public void Load()
    {
        List<Master.SpecialTecniqueData> stData = Master.MasterData.SpecialTecniques;

        for (int i = 0; i < specialTecniques.Length; i++)
        {
            for (int j = 0; j < stData.Count; j++)
            {
                if (specialTecniques[i].m_id == stData[j].id)
                {
                    Master.SpecialTecniqueData d = stData[j];
                    specialTecniques[i].Setting(d.name, d.isSkill, d.type, d.continuationTurn, d.value1, d.value2, d.effects);
                }
            }
        }

        ReleaseSpecialTecnique();
    }

    /// <summary>
    /// �S�Ẵ����N�̃X�e�[�^�X���m�F���ĉ������
    /// </summary>
    public void ReleaseSpecialTecnique()
    {
        for (int t = 0; t < System.Enum.GetValues(typeof(StatusType)).Length; t++)
        {
            StatusType type = (StatusType)System.Enum.ToObject(typeof(StatusType), t);

            for (int r = 0; r < System.Enum.GetValues(typeof(Rank)).Length; r++)
            {
                Rank rank = (Rank)System.Enum.ToObject(typeof(Rank), r);

                if (PlayerDataManager.player.GetRank(type) >= rank)
                {
                    ReleaseSpecialTecnique(rank, type);
                }
            }
        }
    }

    /// <summary>
    /// ����Z�\���
    /// </summary>
    public void ReleaseSpecialTecnique(Rank _rank, StatusType _type)
    {
        Master.CharaInitialStutas status = PlayerDataManager.player.StatusData;

        int id = status.rankPoint.releaseSTId[_rank].GetStatus(_type);
        for(int i = 0; i < specialTecniques.Length; i++)
        {
            if (id == specialTecniques[i].m_id)
            {
                specialTecniques[i].m_released = true;
            }
        }
    }

    /// <summary>
    /// ����Z�\����@��������Z�\�̏���Ԃ�
    /// </summary>
    /// <param name="_rank">�����N</param>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <returns></returns>
    public SpecialTecnique ReleaseSpecialTecniqueAndGetData(Rank _rank, StatusType _type)
    {
        Master.CharaInitialStutas status = PlayerDataManager.player.StatusData;

        int id = status.rankPoint.releaseSTId[_rank].GetStatus(_type);
        for (int i = 0; i < specialTecniques.Length; i++)
        {
            if (id == specialTecniques[i].m_id)
            {
                specialTecniques[i].m_released = true;
                return specialTecniques[i];
            }
        }

        return null;
    }
}