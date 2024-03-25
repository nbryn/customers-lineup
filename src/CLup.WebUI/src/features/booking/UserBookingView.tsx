import React, {useState} from 'react';
import Chip from '@mui/material/Chip';
import {Col, Container, Row} from 'react-bootstrap';
import makeStyles from '@mui/styles/makeStyles';

import {useDeleteUserBookingMutation} from './BookingApi';
import {Header} from '../../shared/components/Texts';
import {MapModal, defaultMapProps} from '../../shared/components/modal/MapModal';
import type {MapModalProps} from '../../shared/components/modal/MapModal';
import type {TableColumn} from '../../shared/components/Table';
import {TableContainer} from '../../shared/containers/TableContainer';
import {useGetUserQuery} from '../user/UserApi';
import {type BookingDto} from '../../autogenerated';

const useStyles = makeStyles(() => ({
    row: {
        justifyContent: 'center',
    },
}));

export const UserBookingView: React.FC = () => {
    const [mapModalInfo, setMapModalInfo] = useState<MapModalProps>(defaultMapProps);
    const styles = useStyles();

    const {data: getUserResponse} = useGetUserQuery();
    const [deleteBookingForUser] = useDeleteUserBookingMutation();

    const columns: TableColumn[] = [
        {title: 'id', field: 'id', hidden: true},
        {title: 'timeSlotId', field: 'timeSlotId', hidden: true},
        {title: 'Date', field: 'date'},
        {title: 'Interval', field: 'interval'},
        {title: 'Business & Zip', field: 'business'},
        {title: 'Street', field: 'street'},
    ];

    const actions = [
        {
            icon: () => <Chip size="small" label="Delete" clickable color="primary" />,
            tooltip: 'Delete Booking',
            onClick: async (_: any, booking: BookingDto) => {
                await deleteBookingForUser(booking.id ?? '');
            },
        },
        {
            icon: () => <Chip size="small" label="See on map" clickable color="secondary" />,
            tooltip: 'Show location on map',
            onClick: (_: any, booking: BookingDto) => {
                setMapModalInfo({
                    visible: true,
                    zoom: 14,
                    center: [booking.longitude as number, booking.latitude as number],
                    markers: [[booking.longitude as number, booking.latitude as number], 13],
                });
            },
        },
    ];

    return (
        <Container>
            <Row className={styles.row}>
                <Header text="Your Bookings" />
            </Row>
            <MapModal
                visible={mapModalInfo.visible}
                setVisible={() => setMapModalInfo(defaultMapProps)}
                zoom={mapModalInfo.zoom}
                center={mapModalInfo.center}
                markers={mapModalInfo.markers}
            />
            <Row className={styles.row}>
                <Col sm={6} md={8} lg={6} xl={10}>
                    <TableContainer
                        actions={actions}
                        columns={columns}
                        tableData={getUserResponse?.user?.bookings ?? []}
                        tableTitle="Bookings"
                        emptyMessage="No Bookings Yet"
                    />
                </Col>
            </Row>
        </Container>
    );
};
