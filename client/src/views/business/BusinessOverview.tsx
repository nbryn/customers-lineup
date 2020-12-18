import {Col, Row} from 'react-bootstrap';
import CircularProgress from '@material-ui/core/CircularProgress';
import {makeStyles} from '@material-ui/core/styles';
import React, {useEffect, useState} from 'react';
import {useHistory} from 'react-router-dom';

import {BUSINESSES_OWNER_URL} from '../../api/URL';
import {BusinessCard} from '../../components/BusinessCard';
import {BusinessDTO} from '../../dto/Business';
import BusinessService, {PathInfo} from '../../services/BusinessService';
import {Header} from '../../components/Texts';
import {RequestHandler, useRequest} from '../../api/RequestHandler';

const useStyles = makeStyles((theme) => ({
   row: {
      justifyContent: 'center',
   },
}));

export const BusinessOverview: React.FC = () => {
   const styles = useStyles();
   const history = useHistory();

   const [businessData, setBusinessData] = useState<BusinessDTO[]>([]);

   const pathInfo: PathInfo = BusinessService.getPathAndTextFromURL(window.location.pathname);

   const requestHandler: RequestHandler<BusinessDTO[]> = useRequest();

   useEffect(() => {
      (async () => {
         const businesses: BusinessDTO[] = await requestHandler.query(BUSINESSES_OWNER_URL);

         setBusinessData(businesses);
      })();
   }, []);

   return (
      <>
         <Row className={styles.row}>
            <Header text="Choose Business" />
         </Row>
         <Row className={styles.row}>
            {requestHandler.working && <CircularProgress />}
            <>
               {businessData.map((x) => {
                  return (
                     <Col key={x.id} sm={6} md={8} lg={4}>
                        <BusinessCard
                           business={x}
                           buttonText={`Manage ${pathInfo.buttonText}`}
                           buttonAction={() =>
                              history.push(`/business/${pathInfo.path}`, {
                                 business: x,
                              })
                           }
                        />
                     </Col>
                  );
               })}
            </>
         </Row>
      </>
   );
};
