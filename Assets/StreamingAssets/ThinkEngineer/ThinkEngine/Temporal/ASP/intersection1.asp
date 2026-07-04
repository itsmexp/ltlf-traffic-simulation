% ASP-LTLf Mapper
%
% INPUT:
% Sensor Fact:
% %intersection1Sensor_detected_car_NS(intersection1,objectIndex(Index),Value).
% %intersection1Sensor_detected_car_SN(intersection1,objectIndex(Index),Value).
% %intersection1Sensor_detected_car_EW(intersection1,objectIndex(Index),Value).
% %intersection1Sensor_detected_car_WE(intersection1,objectIndex(Index),Value).
%
% Self Index Fact:
% currentBrainID(ID).
%
% OUTPUT:
% ltlf_sensor(Output Variable)
%
% Output variable:
% ew_det, ns_det.

ltlf_sensor(ew_det) :- intersection1Sensor_detected_car_EW(intersection1,_,true).
ltlf_sensor(ew_det) :- intersection1Sensor_detected_car_WE(intersection1,_,true).
ltlf_sensor(ns_det) :- intersection1Sensor_detected_car_NS(intersection1,_,true).
ltlf_sensor(ns_det) :- intersection1Sensor_detected_car_SN(intersection1,_,true).