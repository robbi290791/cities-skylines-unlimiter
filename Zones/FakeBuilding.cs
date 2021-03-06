﻿using ColossalFramework;
using ColossalFramework.Math;
using System;
using UnityEngine;
using EightyOne.Attributes;

namespace EightyOne.Zones
{
    /// <summary>
    /// Building is a struct. Calls to this may not work with Replacing the method handle.
    /// </summary>
    internal class FakeBuilding
    {

        [ReplaceMethod]
        public static bool CheckZoning(Building b, ItemClass.Zone zone)
        {            
            int width = b.Width;
            int length = b.Length;
            Vector3 vector3_1 = new Vector3(Mathf.Cos(b.m_angle), 0.0f, Mathf.Sin(b.m_angle));
            Vector3 vector3_2 = new Vector3(vector3_1.z, 0.0f, -vector3_1.x);
            Vector3 vector3_3 = vector3_1 * ((float)width * 4f);
            Vector3 vector3_4 = vector3_2 * ((float)length * 4f);
            Quad3 quad3 = new Quad3();
            quad3.a = b.m_position - vector3_3 - vector3_4;
            quad3.b = b.m_position + vector3_3 - vector3_4;
            quad3.c = b.m_position + vector3_3 + vector3_4;
            quad3.d = b.m_position - vector3_3 + vector3_4;
            Vector3 vector3_5 = quad3.Min();
            Vector3 vector3_6 = quad3.Max();
            int num1= Mathf.Max((int)((vector3_5.x - 46f) / 64f + FakeZoneManager.HALFGRID), 0);
            int num2 = Mathf.Max((int)((vector3_5.z - 46f) / 64f + FakeZoneManager.HALFGRID), 0);
            int num3 = Mathf.Min((int)((vector3_6.x + 46f) / 64f + FakeZoneManager.HALFGRID), FakeZoneManager.GRIDSIZE - 1);
            int num4 = Mathf.Min((int)((vector3_6.z + 46f) / 64f + FakeZoneManager.HALFGRID), FakeZoneManager.GRIDSIZE - 1);
            uint validCells = 0U;
            ZoneManager instance = Singleton<ZoneManager>.instance;
            for (int index1 = num2; index1 <= num4; ++index1)
            {
                for (int index2 = num1; index2 <= num3; ++index2)
                {
                    ushort num5 = FakeZoneManager.zoneGrid[index1 * FakeZoneManager.GRIDSIZE + index2];
                    int num6 = 0;
                    while ((int)num5 != 0)
                    {
                        Vector3 vector3_7 = instance.m_blocks.m_buffer[(int)num5].m_position;
                        if ((double)Mathf.Max(Mathf.Max(vector3_5.x - 46f - vector3_7.x, vector3_5.z - 46f - vector3_7.z), Mathf.Max((float)((double)vector3_7.x - (double)vector3_6.x - 46.0), (float)((double)vector3_7.z - (double)vector3_6.z - 46.0))) < 0.0)
                            CheckZoning(b,zone, ref validCells, ref instance.m_blocks.m_buffer[num5]);
                        num5 = instance.m_blocks.m_buffer[(int)num5].m_nextGridBlock;
                        if (++num6 >= 32768)
                        {
                            CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + System.Environment.StackTrace);
                            break;
                        }
                    }
                }
            }
            for (int index1 = 0; index1 < length; ++index1)
            {
                for (int index2 = 0; index2 < width; ++index2)
                {
                    if (((int)validCells & 1 << (index1 << 3) + index2) == 0)
                        return false;
                }
            }
            return true;
        }

        private static void CheckZoning(Building b, ItemClass.Zone zone, ref uint validCells, ref ZoneBlock block)
        {
            int width = b.Width;
            int length = b.Length;
            Vector3 a = new Vector3(Mathf.Cos(b.m_angle), 0f, Mathf.Sin(b.m_angle)) * 8f;
            Vector3 a2 = new Vector3(a.z, 0f, -a.x);
            int rowCount = block.RowCount;
            Vector3 a3 = new Vector3(Mathf.Cos(block.m_angle), 0f, Mathf.Sin(block.m_angle)) * 8f;
            Vector3 a4 = new Vector3(a3.z, 0f, -a3.x);
            Vector3 a5 = block.m_position - b.m_position + a * ((float)width * 0.5f - 0.5f) + a2 * ((float)length * 0.5f - 0.5f);
            for (int i = 0; i < rowCount; i++)
            {
                Vector3 bb = ((float)i - 3.5f) * a4;
                int num = 0;
                while ((long)num < 4L)
                {
                    if ((block.m_valid & ~block.m_shared & 1uL << (i << 3 | num)) != 0uL && block.GetZone(num, i) == zone)
                    {
                        Vector3 b2 = ((float)num - 3.5f) * a3;
                        Vector3 vector = a5 + b2 + bb;
                        float num2 = a.x * vector.x + a.z * vector.z;
                        float num3 = a2.x * vector.x + a2.z * vector.z;
                        int num4 = Mathf.RoundToInt(num2 / 64f);
                        int num5 = Mathf.RoundToInt(num3 / 64f);
                        if ((num5 != 0 || num == 0) && num4 >= 0 && num5 >= 0 && num4 < width && num5 < length)
                        {
                            validCells |= 1u << (num5 << 3) + num4;
                        }
                    }
                    num++;
                }
            }
        }
    }
}
