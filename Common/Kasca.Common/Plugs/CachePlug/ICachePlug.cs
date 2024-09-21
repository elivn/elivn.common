#region Copyright (C) 2016 Elivn 

/***************************************************************************
*����	�ļ�����������ȫ�ֲ�� -  �������ӿ�
*
*����	�����ˣ� Elivn
*       ������Email��498353921@qq.com
*       
*       
*****************************************************************************/

#endregion

using System;

namespace Kasca.Common.Plugs.CachePlug
{
    /// <summary>
    /// �������ӿ�
    /// </summary>
    public interface ICachePlug
    {
        /// <summary> 
        /// ���ʱ��ι��ڻ���
        /// ������������
        /// </summary>
        /// <typeparam name="T">��ӻ����������</typeparam>
        /// <param name="key">��Ӷ����key</param>
        /// <param name="obj">ֵ</param>
        /// <param name="slidingExpiration">��Թ��ڵ�TimeSpan</param>
        /// <returns>�Ƿ���ӳɹ�</returns>
        bool Set<T>(string key, T obj, TimeSpan slidingExpiration);

        /// <summary>
        /// ��ӹ̶�����ʱ�仺��,������������
        /// </summary>
        /// <typeparam name="T">��ӻ����������</typeparam>
        /// <param name="key">��Ӷ����key</param>
        /// <param name="obj">ֵ</param>
        /// <param name="absoluteExpiration"> ���Թ���ʱ��,��Ϊ�����վ��Թ���ʱ����� </param>
        /// <returns>�Ƿ���ӳɹ�</returns>
        bool Set<T>(string key, T obj, DateTime absoluteExpiration);

        /// <summary>
        /// ��ȡ�������
        /// </summary>
        /// <typeparam name="T">��ȡ�����������</typeparam>
        /// <param name="key">key</param>
        /// <returns>��ȡָ��key��Ӧ��ֵ </returns>
        T Get<T>(string key);

        /// <summary>
        /// �Ƴ��������
        /// </summary>
        /// <param name="key"></param>
        /// <returns>�Ƿ�ɹ�</returns>
        bool Remove(string key);

    }
}