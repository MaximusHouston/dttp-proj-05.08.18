//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================

using System;

namespace FlakeGen
{
    public interface IDistributedIndentifier
    {
        /// <summary>
        /// Unique number generator 32bit
        /// </summary>
        /// <returns></returns>
        int GenerateSequential32Bit();


       /// <summary>
       /// Unique number generator 64bit
       /// </summary>
       /// <returns></returns>
       long GenerateSequential64Bit();


       /// <summary>
       /// Unique number generator Guid
       /// </summary>
       /// <returns></returns>
       Guid GenerateSequentialGuid();

    }
}
