import {BaseService} from './BaseService';
import {BookingDTO} from '../models/Booking';
import {useApi} from '../api/useApi';

const defaultRoute = 'booking';

export interface BookingService extends BaseService {
    createBooking: (timeSlotId: string) => void;
    deleteBookingForBusiness: (timeSlotId: string, userEmail: string) => void;
    deleteBookingForUser: (timeSlotId: string) => void;
    fetchBookingsByBusiness: (businessId: string) => Promise<BookingDTO[]>;
    fetchBookingsByUser: () => Promise<BookingDTO[]>;
}

export function useBookingService(succesMessage?: string): BookingService {
    const apiCaller = useApi(succesMessage);

    const createBooking = (timeSlotId: string): void => {
        apiCaller.post(`${defaultRoute}/${timeSlotId}`);
    };

    const deleteBookingForBusiness = (timeSlotId: string, userEmail: string): void => {
        apiCaller.remove(`${defaultRoute}/business/${timeSlotId}?userEmail=${userEmail}`);
    };

    const deleteBookingForUser = (timeSlotId: string): void => {
        apiCaller.remove(`${defaultRoute}/user${timeSlotId}`);
    };

    const fetchBookingsByBusiness = (businessId: string): Promise<BookingDTO[]> => {
        return apiCaller.get(`${defaultRoute}/business/${businessId}`);
    };

    const fetchBookingsByUser = (): Promise<BookingDTO[]> => {
        return apiCaller.get(`${defaultRoute}/user`);
    };

    const setRequestInfo = (info: string) => apiCaller.setRequestInfo(info);

    return {
        createBooking,
        deleteBookingForBusiness,
        deleteBookingForUser,
        fetchBookingsByBusiness,
        fetchBookingsByUser,
        setRequestInfo,
        requestInfo: apiCaller.requestInfo,
        working: apiCaller.working,
    };
}
