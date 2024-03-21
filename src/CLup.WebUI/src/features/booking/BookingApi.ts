import Cookies from 'js-cookie';

import {BUSINESS_TAG, emptySplitApi, USER_TAG} from '../../app/Store';
import {BookingApi, Configuration, type CreateBookingRequest} from '../../autogenerated';

const bookingApiInstance = new BookingApi(
    new Configuration({accessToken: Cookies.get('access_token')})
);

const bookingApi = emptySplitApi.injectEndpoints({
    endpoints: (builder) => ({
        createBooking: builder.mutation<void, CreateBookingRequest>({
            invalidatesTags: [BUSINESS_TAG, USER_TAG],
            queryFn: async (createBookingRequest) => {
                try {
                    await bookingApiInstance.createBooking(createBookingRequest);
                } catch (error) {
                    return {error} as any;
                }
            },
        }),
        deleteUserBooking: builder.mutation<void, string>({
            invalidatesTags: [BUSINESS_TAG, USER_TAG],
            queryFn: async (bookingId) => {
                try {
                    await bookingApiInstance.deleteUserBooking(bookingId);
                } catch (error) {
                    return {error} as any;
                }
            },
        }),
        deleteBusinessBooking: builder.mutation<void, {businessId: string; bookingId: string}>({
            queryFn: async ({businessId, bookingId}) => {
                try {
                    await bookingApiInstance.deleteBusinessBooking(businessId, bookingId);
                } catch (error) {
                    return {error} as any;
                }
            },
        }),
    }),
});

export const {
    useCreateBookingMutation,
    useDeleteUserBookingMutation,
    useDeleteBusinessBookingMutation,
} = bookingApi;

/* export const selectNextBookingByUser = (state: RootState) =>
    selectBookingsByUser(state)
        .map((booking) => {
            const minutes = booking.interval.substring(
                booking.interval.indexOf(':') + 1,
                booking.interval.indexOf(' ')
            );

            const hours = booking.interval.substring(0, booking.interval.indexOf(':'));
            const day = booking.date.substring(0, booking.date.indexOf('/'));

            const month = booking.date.substring(
                booking.date.indexOf('/') + 1,
                booking.date.lastIndexOf('/')
            );

            const year = booking.date.substring(
                booking.date.lastIndexOf('/') + 1,
                booking.date.length
            );

            const date = new Date(
                parseInt(year),
                parseInt(month) - 1,
                parseInt(day) - 1,
                parseInt(hours),
                parseInt(minutes)
            );

            return {booking, date, dateString: booking.date + ' - ' + hours + ':' + minutes};
        })
        .sort((entry1, entry2) => +entry1.date - +entry2.date)[0]; */