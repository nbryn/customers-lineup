import React, {useState} from 'react';
import Chip from '@material-ui/core/Chip';
import {Col, Container, Row} from 'react-bootstrap';
import {makeStyles} from '@material-ui/core/styles';

import {BookingDTO} from './Booking';
import {deleteBookingForUser, selectBookingsByUser} from './BookingState';
import {Header} from '../../shared/components/Texts';
import {MapModal, MapModalProps, defaultMapProps} from '../../shared/components/modal/MapModal';
import {TableColumn} from '../../shared/components/Table';
import {TableContainer} from '../../shared/containers/TableContainer';
import {useAppDispatch, useAppSelector} from '../../app/Store';

const useStyles = makeStyles((theme) => ({
    row: {
        justifyContent: 'center',
    },
}));

export const UserBookingView: React.FC = () => {
    const styles = useStyles();
    const dispatch = useAppDispatch();

    const bookings = useAppSelector(selectBookingsByUser);
    const [mapModalInfo, setMapModalInfo] = useState<MapModalProps>(defaultMapProps);

    const columns: TableColumn[] = [
        {title: 'id', field: 'id', hidden: true},
        {title: 'timeSlotid', field: 'timeSlotId', hidden: true},
        {title: 'Date', field: 'date'},
        {title: 'Interval', field: 'interval'},
        {title: 'Business & Zip', field: 'business'},
        {title: 'Street', field: 'street'},      
    ];

    const actions = [
        {
            icon: () => <Chip size="small" label="Delete" clickable color="primary" />,
            tooltip: 'Delete Booking',
            onClick: async (event: any, booking: BookingDTO) => {
                dispatch(deleteBookingForUser(booking.id));
            },
        },
        {
            icon: () => <Chip size="small" label="See on map" clickable color="secondary" />,
            tooltip: 'Show location on map',
            onClick: (event: any, booking: BookingDTO) => {
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
                        tableData={bookings}
                        tableTitle="Bookings"
                        emptyMessage="No Bookings Yet"
                    />
                </Col>
            </Row>
        </Container>
    );
};