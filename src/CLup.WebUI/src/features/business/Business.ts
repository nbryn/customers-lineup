import type {Address} from '../../shared/services/AddressService';
import type {DTO, HasAddress} from '../../shared/models/General';
import type {BookingDTO} from '../booking/Booking';
import type {EmployeeDTO} from './employee/Employee';
import type {MessageDTO} from '../message/Message';
import type {TimeSlotDTO} from './timeslot/TimeSlot';

export interface BusinessDTO extends DTO, HasAddress {
    name: string;
    type: string;
    timeSlotLength: number | string;
    capacity: number | string;
    opens: string;
    closes: string;
    businessHours?: string;
    ownerEmail?: string;
    bookings?: BookingDTO[];
    employees?: EmployeeDTO[];
    messages?: MessageDTO[];
    timeSlots?: TimeSlotDTO[];
}

export type BusinessDataDTO = {
    numberOfBookings: number;
    numberOfTimeSlots: number;
    numberOfEmployees?: number;
};

export function updateBusiness(
    updatedBusiness: BusinessDTO,
    business: BusinessDTO,
    address: Address
) {
    updatedBusiness.longitude = address?.longitude ?? business.longitude;
    updatedBusiness.latitude = address?.latitude ?? business.latitude;
    updatedBusiness.city = address?.city ?? business.city;
    updatedBusiness.zip = address?.zip ?? business.zip;

    updatedBusiness.opens = updatedBusiness.opens.replace(':', '.');
    updatedBusiness.closes = updatedBusiness.closes.replace(':', '.');

    return updatedBusiness;
}
