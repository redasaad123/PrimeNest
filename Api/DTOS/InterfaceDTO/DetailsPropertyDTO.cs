﻿namespace ProjectApi.DTOS.InterfaceDTO
{
    public class DetailsPropertyDTO
    {
        public DetailsPropertyDTO()
        {   
        }

        public string PropertyId { get; set; }
        public string type { get; set; }
        public int? NumberOfRooms { get; set; }
        public int? NumberOfBathrooms { get; set; }
        public int? NumberStorey { get; set; }
        public string? PoolArea { get; set; }
        public string? gardenArea { get; set; }
        public string? TypeFloor { get; set; }

    }
}
