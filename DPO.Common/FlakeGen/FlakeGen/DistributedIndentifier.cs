// Copyright (c) 2012 - Jeremiah Peschka
//
// This file is provided to you under the Apache License,
// Version 2.0 (the "License"); you may not use this file
// except in compliance with the License.  You may obtain
// a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.

using System;
using System.Linq;
using System.Net.NetworkInformation;
using FlakeGen;

namespace FlakeGen
{
   /// <summary>
   /// Initializes the repository for SQLCE
   /// </summary>
   public class DistributedIndentifier : IDistributedIndentifier
   {

      #region Properties

      private IIdGenerator<int> idGenerator32;
      private IIdGenerator<long> idGenerator64;
      private IIdGenerator<Guid> idGeneratorGuid;

      private static int MachineIdentifier = UseNetworkToGetUniqueNumber();

      #endregion

      public DistributedIndentifier()
      {
         idGenerator32 = new Id32Generator(MachineIdentifier & 3);
         idGenerator64 = new Id64Generator(MachineIdentifier);
         idGeneratorGuid = new IdGuidGenerator(MachineIdentifier);
      }

      public static int UseNetworkToGetUniqueNumber()
      {
         NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

         byte[] uniqueAddress = Guid.NewGuid().ToByteArray();

         foreach (NetworkInterface adapter in nics)
         {

            if ((adapter.NetworkInterfaceType == NetworkInterfaceType.GigabitEthernet ||
                     adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                     adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet3Megabit ||
                     adapter.NetworkInterfaceType == NetworkInterfaceType.FastEthernetFx ||
                     adapter.NetworkInterfaceType == NetworkInterfaceType.FastEthernetT ||
                     adapter.NetworkInterfaceType == NetworkInterfaceType.Fddi)
                  && adapter.OperationalStatus == OperationalStatus.Up)
            {
               uniqueAddress = adapter.GetPhysicalAddress().GetAddressBytes();
               break;
            }

         }
         return (int)(BitConverter.ToInt32(uniqueAddress, 0) & 31);
      }

      public int GenerateSequential32Bit()
      {

         return idGenerator32.GenerateId();
      }

      public long GenerateSequential64Bit()
      {

          return idGenerator64.GenerateId();
      }

      public Guid GenerateSequentialGuid()
      {
         return idGeneratorGuid.GenerateId();
      }
   }
}
