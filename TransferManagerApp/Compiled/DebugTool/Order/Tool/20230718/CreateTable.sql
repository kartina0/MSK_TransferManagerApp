\set pick_head_name dp01_pick_head_0
\set pick_detail_name dp02_pick_detail_0
\set comp_head_name dp11_comp_head_0
\set comp_detail_name dp12_comp_detail_0

\echo :pick_head_name:name_date
CREATE TABLE :pick_head_name:name_date (
	delivery_date			varchar(8),
	post_no					varchar(1)		NOT NULL,
	delivery_date_order		varchar(8),
	post_no_order			varchar(1)		NOT NULL,
	sku_code				varchar(6)		NOT NULL,
	pd_count				numeric(4,0)	NOT NULL,
	sku_name				varchar(22),
	jan_code				varchar(13),
	case_volume				numeric(5,0),
	pieces_num_total		numeric(7,0),
	sku_kana				varchar(22),
	max_stack_num			varchar(1),
	sales_price				numeric(6,0),
	pick_class				varchar(1),
	pieces_num_st1			numeric(7,0),
	pieces_num_st2			numeric(7,0),
	pieces_num_st3			numeric(7,0),
	pieces_num_st4			numeric(7,0),
	pieces_num_st5			numeric(7,0),
	pieces_num_st6			numeric(7,0),
	pieces_num_st7			numeric(7,0),
	pieces_num_st8			numeric(7,0),
	pieces_num_st9			numeric(7,0),
	store_num_st1			numeric(4,0),
	store_num_st2			numeric(4,0),
	store_num_st3			numeric(4,0),
	store_num_st4			numeric(4,0),
	store_num_st5			numeric(4,0),
	store_num_st6			numeric(4,0),
	store_num_st7			numeric(4,0),
	store_num_st8			numeric(4,0),
	store_num_st9			numeric(4,0),
	create_date				varchar(8)		DEFAULT 0,
	create_time				varchar(6)		DEFAULT 0,
	create_login_id			varchar(10),
	renew_date				varchar(8)		DEFAULT 0,
	renew_time				varchar(6)		DEFAULT 0,
	renew_login_id			varchar(10),
	PRIMARY KEY(delivery_date, post_no, delivery_date_order, post_no_order, sku_code, pd_count)
);

\echo :pick_detail_name:name_date
CREATE TABLE :pick_detail_name:name_date (
	delivery_date			varchar(8),
	post_no					varchar(1)		NOT NULL,
	delivery_date_order		varchar(8),
	post_no_order			varchar(1)		NOT NULL,
	sku_code				varchar(6)		NOT NULL,
	pd_count				numeric(4,0)	NOT NULL,
	store_code				varchar(6)		NOT NULL,
	station_no				varchar(1),
	aisle_no				varchar(2),
	slot_no					varchar(2),
	case_volume				numeric(5,0),
	pieces_num				numeric(7,0),
	create_date				varchar(8)		DEFAULT 0,
	create_time				varchar(6)		DEFAULT 0,
	create_login_id			varchar(10),
	renew_date				varchar(8)		DEFAULT 0,
	renew_time				varchar(6)		DEFAULT 0,
	renew_login_id			varchar(10),
	PRIMARY KEY(delivery_date, post_no, delivery_date_order, post_no_order, sku_code, pd_count, store_code)
);

\echo :comp_head_name:name_date
CREATE TABLE :comp_head_name:name_date (
	delivery_date			varchar(8),
	post_no					varchar(1)		NOT NULL,
	delivery_date_order		varchar(8),
	post_no_order			varchar(1)		NOT NULL,
	sku_code				varchar(6)		NOT NULL,
	pd_count				numeric(4,0)	NOT NULL,
	jan_code				varchar(13),
	pieces_num_total		numeric(7,0),
	comp_num_total			numeric(7,0),
	slip_date				varchar(14),
	create_date				varchar(8)		DEFAULT 0,
	create_time				varchar(6)		DEFAULT 0,
	create_login_id			varchar(10),
	renew_date				varchar(8)		DEFAULT 0,
	renew_time				varchar(6)		DEFAULT 0,
	renew_login_id			varchar(10),
	PRIMARY KEY(delivery_date, post_no, delivery_date_order, post_no_order, sku_code, pd_count)
);

\echo :comp_detail_name:name_date
CREATE TABLE :comp_detail_name:name_date (
	delivery_date			varchar(8),
	post_no					varchar(1)		NOT NULL,
	delivery_date_order		varchar(8),
	post_no_order			varchar(1)		NOT NULL,
	sku_code				varchar(6)		NOT NULL,
	pd_count				numeric(4,0)	NOT NULL,
	store_code				varchar(6)		NOT NULL,
	station_no				varchar(1),
	aisle_no				varchar(2),
	slot_no					varchar(2),
	pieces_num				numeric(7,0),
	comp_num				numeric(7,0),
	create_date				varchar(8)		DEFAULT 0,
	create_time				varchar(6)		DEFAULT 0,
	create_login_id			varchar(10),
	renew_date				varchar(8)		DEFAULT 0,
	renew_time				varchar(6)		DEFAULT 0,
	renew_login_id			varchar(10),
	PRIMARY KEY(delivery_date, post_no, delivery_date_order, post_no_order, sku_code, pd_count, store_code)
);
