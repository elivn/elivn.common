using System;
using Kasca.Common.ComModels.Enums;

namespace Kasca.Common.ComModels
{
    public class BaseMo<TPrimaryKey>
    {
        public TPrimaryKey Id { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }
    }

    public class BaseMo : BaseMo<long>
    {

    }

    public class BaseStateMo
    {
        public CommonStatus Status { get; set; }
    }


    public class BaseStateMo<IdType> : BaseMo<IdType>
    {
        public CommonStatus Status { get; set; }
    }

}
