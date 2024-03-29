import {BUSINESS_TAG, baseApi, USER_TAG} from '../../app/Store';
import {BookingApi, type CreateBookingRequest} from '../../autogenerated';
import {apiMutation} from '../../shared/api/Api';

const BUSINESS_DELETED_BOOKING_MSG = 'Booking Deleted - User has been notified';
const USER_DELETED_BOOKING_MSG = 'Booking Deleted';

const BOOKING_CREATED_SUCCESS_INFO = {
    message: 'Success - Go to my bookings to see your bookings',
    toastInfo: {buttonText: 'My Bookings', navigateTo: '/user/bookings'},
};

const bookingApi = baseApi.injectEndpoints({
    endpoints: (builder) => ({
        createBooking: builder.mutation<void, CreateBookingRequest>({
            invalidatesTags: [BUSINESS_TAG, USER_TAG],
            queryFn: async (createBookingRequest, api) => ({
                data: await apiMutation(
                    async (bookingApi) => await bookingApi.createBooking(createBookingRequest),
                    BookingApi,
                    api,
                    BOOKING_CREATED_SUCCESS_INFO
                ),
            }),
        }),
        deleteUserBooking: builder.mutation<void, string>({
            invalidatesTags: [BUSINESS_TAG, USER_TAG],
            queryFn: async (bookingId, api) => ({
                data: await apiMutation(
                    async (bookingApi) => await bookingApi.deleteUserBooking(bookingId),
                    BookingApi,
                    api,
                    {message: USER_DELETED_BOOKING_MSG}
                ),
            }),
        }),
        deleteBusinessBooking: builder.mutation<void, {businessId: string; bookingId: string}>({
            queryFn: async ({businessId, bookingId}, api) => ({
                data: await apiMutation(
                    async (bookingApi) =>
                        await bookingApi.deleteBusinessBooking(businessId, bookingId),
                    BookingApi,
                    api,
                    {message: BUSINESS_DELETED_BOOKING_MSG}
                ),
            }),
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
