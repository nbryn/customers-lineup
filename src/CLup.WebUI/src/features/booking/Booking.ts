import {DTO} from '../../shared/models/General';

export interface BookingDTO extends DTO {
    timeSlotId: string;
    businessId: string;
    userId: string;
    street: string;
    business: string;
    longitude: number;
    latitude: number;
    userEmail: string;
    date: string;
    capacity: string;
    interval: string;
}